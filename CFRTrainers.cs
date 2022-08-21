using ScottPlot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyPokerSolver
{
    //TO DO - move else where
    /// <summary>
    /// 
    /// NOTE: NODE UTILITY IS ALWAYS FROM PLAYER 1 PERSPECTIVE, METHODS WITHIN GAMESTATE CLASS SORT IT OUT
    /// 
    /// </summary>
    public class VanillaCFRTrainer
    {
        public Dictionary<string, InformationSet> InfoSetMap { get; set; }

        public BestResponseUtility BestResponseUtility { get; set; }

        public InformationSetCFRLogic InformationSetMethods { get; set; }

        public int Iteration { get; set; }
        public Player UpdatingPlayer { get; set; }


        public VanillaCFRTrainer()
        {
            InfoSetMap = new Dictionary<string, InformationSet>();
        }

        public InformationSet GetInformationSet(GameStateNode gameStateNode)
        {
            List<string> infoSetHistory = new List<string>(gameStateNode.History);

            if (gameStateNode.ActivePlayer == Player.Player1)
            {
                infoSetHistory.InsertRange(0, gameStateNode.Player1Cards);
            }
            else if (gameStateNode.ActivePlayer == Player.Player2)
            {
                infoSetHistory.InsertRange(0, gameStateNode.Player2Cards);
            }

            string key = string.Join("_", infoSetHistory);

            if (InfoSetMap.ContainsKey(key) == false)
            {
                InfoSetMap[key] = new InformationSet(gameStateNode.ActionOptions.Count);
            }

            return InfoSetMap[key];
        }


        public float Train(int numberIterations, List<string> boardArranged, List<string[]> handCombosP1, List<string[]> handCombosP2)
        {
            InformationSetMethods = new InformationSetCFRLogic();
            BestResponseUtility = new BestResponseUtility(InfoSetMap, InformationSetMethods);
            Iteration = 0;

            float P1Util = 0;
            int utilP1Count = 0;

            List<string> startHistory = new List<string>(boardArranged);

            for (int i = 0; i < numberIterations; i++)
            {
                Iteration = i;
                UpdatingPlayer = (Player) (i % 2);

                P1Util = 0;
                utilP1Count = 0;

                //Iterate through all possible hand combinations
                for (int indexP1 = 0; indexP1 < handCombosP1.Count; indexP1++)
                {
                    //Dont include p1 hands that conflict with the board
                    if (boardArranged.Contains(handCombosP2[indexP1][0]) || boardArranged.Contains(handCombosP2[indexP1][1]))
                    {
                        continue;
                    }

                    for (int indexP2 = 0; indexP2 < handCombosP2.Count; indexP2++)
                    {
                        //Dont include p2 hands that conflict with curren p1 hands
                        if (handCombosP2[indexP2].Contains(handCombosP1[indexP1][0]) || handCombosP2[indexP2].Contains(handCombosP1[indexP1][1]))
                        {
                            continue;
                        }
                        //Dont include p2 hands that conflict with the board
                        if(boardArranged.Contains(handCombosP2[indexP2][0]) || boardArranged.Contains(handCombosP2[indexP2][1]))
                        {
                            continue;
                        }

                        //Initialise startNode
                        GameStateNode startNode = GameStateNode.GetStartingNode(startHistory, handCombosP1[indexP1].ToList(), handCombosP2[indexP2].ToList());

                        //Begin the CFR Recursion
                        P1Util += CalculateNodeUtility(startNode);
                        utilP1Count++;
                    }
                }

                Console.WriteLine($"Iteration {i} complete.");
                Console.WriteLine($"Strategy Exploitability Percentage: {BestResponseUtility.TotalDeviation(boardArranged, handCombosP1, handCombosP2)}");
                Console.WriteLine();
            }

            //return player 1 utility of last iteration
            return P1Util / utilP1Count;
        }

        //Returns utility from player 1 perspective
        public float CalculateNodeUtility(GameStateNode gameStateNode)
        {
            ///// TERMINAL NODE /////
            if (gameStateNode.ActivePlayer == Player.GameEnd)
            {
                var u = PokerRules.CalculatePayoff(gameStateNode.History, gameStateNode.Player1Cards, gameStateNode.Player2Cards);

                return u;
            }


            float[] actionUtilities = new float[gameStateNode.ActionOptions.Count];
            float nodeUtility;

            ///// CHANCE NODE /////
            if (gameStateNode.ActivePlayer == Player.ChancePublic)
            {
                float actionProbability = (float)1 / gameStateNode.ActionOptions.Count;

                List<Task<float>> tasks = new List<Task<float>>();

                //get utility of each action
                for (int i = 0; i < actionUtilities.Length; i++)
                {
                    GameStateNode childGameState = new GameStateNode(gameStateNode, gameStateNode.ActionOptions[i], actionProbability);
                    actionUtilities[i] = CalculateNodeUtility(childGameState);
                }

                //average utility for node calculated by Dot product action utilities and action probabilities
                nodeUtility = actionUtilities.Select(u => u * actionProbability).Sum();

                return nodeUtility;
            }

            ///// DECISION NODE /////
            else
            {
                float activePlayerReachProbability;

                if (gameStateNode.ActivePlayer == Player.Player1)
                {
                    activePlayerReachProbability = gameStateNode.ReachProbabiltyP1;
                }
                else //ActivePlayer == Player2
                {
                    activePlayerReachProbability = gameStateNode.ReachProbabiltyP2;
                }


                InformationSet infoSet = GetInformationSet(gameStateNode);

                var strategy = InformationSetMethods.GetStrategy(infoSet);

                //Only update on updating player
                if (gameStateNode.ActivePlayer == UpdatingPlayer)
                {
                    InformationSetMethods.AddToStrategySum(infoSet, strategy, activePlayerReachProbability);
                }




                //get utility of each action
                for (int i = 0; i < actionUtilities.Length; i++)
                {
                    var actionProbability = strategy[i];
                    GameStateNode childGameState = new GameStateNode(gameStateNode, gameStateNode.ActionOptions[i], actionProbability);
                    actionUtilities[i] = CalculateNodeUtility(childGameState);
                }

                //average utility for node calculated by Dot product action utilities and action probabilities
                nodeUtility = actionUtilities.Zip(strategy, (x, y) => x * y).Sum();

                //Only update on updating player
                if (gameStateNode.ActivePlayer == UpdatingPlayer)
                {
                    InformationSetMethods.AddToCumulativeRegrets(infoSet, gameStateNode, actionUtilities, nodeUtility);
                }

                return nodeUtility;
            }
        }
    }
}

