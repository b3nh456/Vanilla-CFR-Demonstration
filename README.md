<h1>Vanilla CFR Demonstration<h1>
This repository is for a simple demonstration of vanilla counterfactual regret minimisation algorithm

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


After running all the iterations a primitive CLI will be available to view the strategies from the algorithm
KEY:
C0 = Check or Call dependant on context
F0 = Fold
R0 = Raise by amount of bet size 1
R1 = Raise by amount of bet size 2
R2 = etc.

Output will be of the form:
[history] [Stategy]

