<p1>
<h1>Vanilla CFR Demonstration</h1>

This repository is for a simple demonstration of vanilla counterfactual regret minimisation algorithm

<h3>Inputs </h3>
Within Program Main you can manually change the following inputs:
-Number of iterations
-Starting Board
-Player 1 Range
-Player 2 Range
-Start pot size
-Effective stack size
-Available bet sizes (as a percentage of the pot)

Note: Due to this being designed as a demonstration the code and algorithm are very slow, and will take a long
time to run through any sizable game tree.

<h3> CLI </h3>
After running all the iterations a primitive CLI will be available to view the strategies from the algorithm

Action Key:
C0 = Check or Call dependant on context
F0 = Fold
R0 = Raise by amount of bet size 1
R1 = Raise by amount of bet size 2
R2 = etc.
S0 = All in

Card Key:
Card Rank: 2-9,T,J,Q,K,A
Card Suit: s=spades, d=diamonds, h=hearts, c=clubs

Output will be of the form:
[history] [Stategy]

EG. A history of "Ad_As_2d_3s_5c_R0_R0"
Will output the strategy of player 1 who has a hand of Ace diamonds, Ace spades after a 2d,3s,5c flop and after raising and then being re-raised
</p1>

