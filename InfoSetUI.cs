using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPokerSolver
{
    public static class InfoSetUI
    {
        public static void View(VanillaCFRTrainer trainer, int startBoardSize)
        {
            InformationSetCFRLogic UpdaterInfoSet = trainer.InformationSetMethods;
            int startReadIndex = (startBoardSize + 2) * 3;
            bool exit = false;
            string infoSetAction="";
            while (exit == false)
            {
                Console.WriteLine("\n\n=========================================================");
                Console.WriteLine("Input Action For Information Sets You Would Like to View");
                Console.WriteLine("E.G \"C0_C0_Ks_R0\" to view check->check->kingspades->bet");
                Console.WriteLine("Put \"+\" to add to previous command and \"quit\" to exit");
                Console.Write("INPUT: "); string command = Console.ReadLine();

                if (command == "exit" || command == "quit")
                {
                    exit = true;
                    break;
                }
                if (command.Length>=1 && command[0] == '+')
                {
                    infoSetAction = infoSetAction + command.Remove(0,1) ;
                }
                else
                {
                    infoSetAction = command;
                }


                bool showActions = true;
                foreach (string key in trainer.InfoSetMap.Keys)
                {

                    //Edge case where no action (eg first person to act on flop)
                    if (infoSetAction == "")
                    {
                        if (key.Length <= startReadIndex)
                        {
                            if (showActions)
                            {
                                List<string> actions = PokerRules.AvailablePlayerActions(key.Split('_').ToList());
                                Console.WriteLine($"        === ACTIONS: {string.Join(", ", actions )} === ");
                                showActions = false;
                            }
                            Console.WriteLine($"{key}  STRATEGY: {string.Join(", ", UpdaterInfoSet.GetFinalStrategy(trainer.InfoSetMap[key])) }");
                        }
                        continue;
                    }


                    if (key.Length > startReadIndex && key.Length == startReadIndex + infoSetAction.Length && key.IndexOf(infoSetAction, startReadIndex, infoSetAction.Length) == startReadIndex)
                    {
                        if (showActions)
                        {
                            List<string> actions = PokerRules.AvailablePlayerActions(key.Split('_').ToList());
                            Console.WriteLine($"=== ACTIONS: {string.Join(", ", actions)}");
                            showActions = false;
                        }
                        Console.WriteLine($"{key}  STRATEGY: {string.Join(", ", UpdaterInfoSet.GetFinalStrategy(trainer.InfoSetMap[key])) }");
                    }
                }
            }
        }
    }
}
