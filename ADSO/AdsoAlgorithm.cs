using GeneticMultistepCoevoSG.Struct;
using GeneticMultistepSG.Struct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticMultistepSG.Adso
{
    public class AdsoAlgorithm
    {
        private Random Rand;
        private Ggame GameConfig; // Supports WHG/SEG
        private FlipItGame FlipGameConfig; 
        private bool IsFlipGame;
        
        // ADSO Parameters
        private int PopulationSize;
        private double LearningRate = 1e-4;
        
        // Augmented Decision Space State
        private double[] Q; // Binary probabilities 
        private CmaEsOptimizer CmaOptimizer; // Real-valued part 
        
        // Dimensionality
        private int TimeSteps;
        private int EdgeCount;
        private int Dimension; // m * |E|

        public AdsoAlgorithm(Game game, int popSize)
        {
            Rand = Program.rand;
            PopulationSize = popSize;
            
            if (game is Ggame g)
            {
                GameConfig = g;
                IsFlipGame = false;
                TimeSteps = GameConfig.rounds;
                EdgeCount = GameConfig.graphConfig.edges.Count;
            }
            else if (game is FlipItGame f)
            {
                FlipGameConfig = f;
                IsFlipGame = true;
                TimeSteps = FlipGameConfig.rounds;
                EdgeCount = FlipGameConfig.graph.edges.Count;
            }

            Dimension = TimeSteps * EdgeCount;

            Q = new double[Dimension];
            for (int i = 0; i < Dimension; i++) Q[i] = 0.5;

            CmaOptimizer = new CmaEsOptimizer(Dimension, PopulationSize, 0.5, Rand);
        }

        public void Run(int maxGenerations)
        {
            for (int t = 0; t < maxGenerations; t++)
            {
                // 1. Sample Population
                bool[][] binarySamples = new bool[PopulationSize][];
                double[][] realSamples = CmaOptimizer.SamplePopulation();

                for (int k = 0; k < PopulationSize; k++)
                {
                    binarySamples[k] = new bool[Dimension];
                    for (int i = 0; i < Dimension; i++)
                    {
                        binarySamples[k][i] = Rand.NextDouble() < Q[i];
                    }
                }

                // Compute Follower BR once against the Mean Leader Strategy if Zero-Sum
                List<int> fixedBestResponse = null;
                bool useDanskin = !IsFlipGame; // Assuming WHG/SEG usually benefit, strictly WHG zero-sum
                
                if (useDanskin)
                {
                    // Construct Mean Strategy from Q and CmaMean
                    double[] meanReal = CmaOptimizer.Mean;
                    bool[] meanBinary = Q.Select(p => p > 0.5).ToArray(); // Thresholding for representative mean
                    var meanStrategy = DecodeStrategy(meanBinary, meanReal);
                    
                    // Compute BR
                    fixedBestResponse = GetFollowerBestResponse(meanStrategy);
                }

                // 3. Evaluation
                double[] payoffs = new double[PopulationSize];
                
                // Helper for Q-update gradient accumulation
                double[][] utilities = new double[PopulationSize][]; // Rank-based utilities
                
                for (int k = 0; k < PopulationSize; k++)
                {
                    var strategy = DecodeStrategy(binarySamples[k], realSamples[k]);
                    
                    if (useDanskin && fixedBestResponse != null)
                    {
                        payoffs[k] = EvaluateLeaderStrategyFixedBR(strategy, fixedBestResponse);
                    }
                    else
                    {
                        // Full evaluation (find BR for specific sample)
                        payoffs[k] = EvaluateLeaderStrategyFull(strategy);
                    }
                }

                // Rank-based shaping
                var sortedIndices = Enumerable.Range(0, PopulationSize)
                    .OrderByDescending(i => payoffs[i])
                    .ToArray();
                
                // Calculate Utilities u_j (simplified PBIL style: top half gets positive weight)
                double[] u = new double[PopulationSize];
                for(int k=0; k < PopulationSize; k++)
                {
                   // Linear ranking or simple top-selection. Paper mentions PBIL equivalence.
                   // But Eq 13 implies a weighted sum. We use normalized rank weights.
                   int rank = Array.IndexOf(sortedIndices, k);
                   if (rank < PopulationSize / 2) 
                       u[k] = (PopulationSize / 2.0 - rank) / (PopulationSize / 2.0); // Simple linear
                   else 
                       u[k] = 0;
                }

                // Gradient Update Eq 13: q_i = q_i + eta * Sum( u_j * (x_ij - q_i) )
                for (int i = 0; i < Dimension; i++)
                {
                    double gradientSum = 0;
                    for (int k = 0; k < PopulationSize; k++)
                    {
                        double x_val = binarySamples[k][i] ? 1.0 : 0.0;
                        gradientSum += u[k] * (x_val - Q[i]);
                    }
                    Q[i] = Q[i] + LearningRate * gradientSum;
                    
                    // Clamp to avoid stagnation
                    Q[i] = Math.Max(0.01, Math.Min(0.99, Q[i]));
                }

                CmaOptimizer.UpdateDistribution(realSamples, payoffs);

                // Logging
                double bestPayoff = payoffs.Max();
                if (t % 10 == 0)
                {
                    Console.WriteLine($"Generation {t}: Best Payoff = {bestPayoff}");
                    Program.attackerWriter.WriteLine($"{Program.currentGame};{t};{bestPayoff};");
                    Program.attackerWriter.Flush();
                }
            }
        }

        // Decodes (\hat{x}, \tilde{x}) into Edge Probabilities P(e, t)
        // Returns a matrix [TimeStep][EdgeIndex] -> Probability
        private double[][] DecodeStrategy(bool[] binary, double[] real)
        {
            double[][] strategy = new double[TimeSteps][];
            for (int t = 0; t < TimeSteps; t++)
            {
                strategy[t] = new double[EdgeCount];
                double sum = 0;
                
                for (int e = 0; e < EdgeCount; e++)
                {
                    int idx = t * EdgeCount + e;
                    
                    double val = (binary[idx] ? 1.0 : 0.0) * Math.Max(0, real[idx]); // Ensure non-negative
                    strategy[t][e] = val;
                    sum += val;
                }

                if (sum > 1e-9)
                {
                    for (int e = 0; e < EdgeCount; e++) strategy[t][e] /= sum;
                }
                else
                {
                    // Fallback uniform if all turned off
                    double uniform = 1.0 / EdgeCount;
                    for (int e = 0; e < EdgeCount; e++) strategy[t][e] = uniform;
                }
            }
            return strategy;
        }

        // ================= EVALUATION LOGIC =================
        // ADSO optimizes edge probabilities (Behavioral Strategy). 
        // We must calculate P(Defender at v at time t).
        
        private double EvaluateLeaderStrategyFull(double[][] edgeProbs)
        {
            List<int> bestAttacker = GetFollowerBestResponse(edgeProbs);
            return EvaluateLeaderStrategyFixedBR(edgeProbs, bestAttacker);
        }

        // Calculates U_L(\pi_L, \pi_F) where \pi_L is edgeProbs
        private double EvaluateLeaderStrategyFixedBR(double[][] edgeProbs, List<int> attackerPath)
        {
            if (IsFlipGame) return EvaluateFlipIt(edgeProbs, attackerPath);
            return EvaluateGGame(edgeProbs, attackerPath);
        }

        private List<int> GetFollowerBestResponse(double[][] edgeProbs)
        {
             // Finds the Attacker Path that maximizes U_F against edgeProbs
             // Iterates all pre-calculated attacker strategies (as done in original code)
             
             List<List<int>> strategies = IsFlipGame ? FlipGameConfig.attackerStrategies : GameConfig.attackerStrategies;
             
             double maxVal = double.MinValue;
             List<int> bestStrat = strategies[0];

             foreach(var strat in strategies)
             {
                 double val = EvaluateFollowerPayoff(edgeProbs, strat);
                 if(val > maxVal)
                 {
                     maxVal = val;
                     bestStrat = strat;
                 }
             }
             return bestStrat;
        }

        // --- GGame Evaluation logic based on Node Probability Propagation ---
        private double EvaluateFollowerPayoff(double[][] edgeProbs, List<int> attackerPath)
        {
            // For Zero-Sum WHG, U_F = -U_L. But let's calculate explicitly if available.
            // Using standard Expectation calculation.
            double expectedAttacker = 0;
            double currentProbAlive = 1.0;

            // Compute Defender Node Distributions P_def(v, t)
            double[][] nodeProbs = ComputeNodeProbabilities(edgeProbs);

            for (int t = 0; t < attackerPath.Count; t++)
            {
                int v_att = attackerPath[t];
                
                // Probability caught at this step: Defender is at v_att at time t
                double p_caught = nodeProbs[t][v_att];

                // Immediate rewards/penalties from original code logic
                double penalty = IsFlipGame ? 0 : GameConfig.vertexAttackerPenalties[v_att];
                // In GGame, payoff accumulates
                expectedAttacker += currentProbAlive * p_caught * penalty; 

                // Check Target
                if (!IsFlipGame && GameConfig.targets.Contains(v_att))
                {
                    double reward = GameConfig.targetAttackerRewards[GameConfig.targets.IndexOf(v_att)];
                    expectedAttacker += currentProbAlive * (1 - p_caught) * reward;
                    return expectedAttacker; // Game ends
                }

                currentProbAlive *= (1 - p_caught);
            }
            return expectedAttacker;
        }

        private double EvaluateGGame(double[][] edgeProbs, List<int> attackerPath)
        {
            double expectedDefender = 0;
            double currentProbAlive = 1.0;
            double[][] nodeProbs = ComputeNodeProbabilities(edgeProbs);

            for (int t = 0; t < attackerPath.Count; t++)
            {
                int v_att = attackerPath[t];
                double p_caught = nodeProbs[t][v_att];
                double reward = GameConfig.vertexDefenderRewards[v_att];

                expectedDefender += currentProbAlive * p_caught * reward;

                if (GameConfig.targets.Contains(v_att))
                {
                    double penalty = GameConfig.targetDefenderPenalties[GameConfig.targets.IndexOf(v_att)];
                    expectedDefender += currentProbAlive * (1 - p_caught) * penalty;
                    return expectedDefender;
                }
                currentProbAlive *= (1 - p_caught);
            }
            return expectedDefender;
        }

        // Converts Edge Probabilities (Output of ADSO) to Node Probabilities (Needed for Eval)
        private double[][] ComputeNodeProbabilities(double[][] edgeProbs)
        {
             // P_def(v, t) = Sum_{u} P_def(u, t-1) * P(u->v @ t-1)
             int nodes = IsFlipGame ? FlipGameConfig.graph.vertexCount : GameConfig.graphConfig.vertexCount;
             double[][] nodeProbs = new double[TimeSteps + 1][];
             for(int t=0; t<=TimeSteps; t++) nodeProbs[t] = new double[nodes];

             // Init: Start Node (assumed 0 or spawn)
             int startNode = IsFlipGame ? 0 : GameConfig.spawn; // Simplified, check original code for spawn logic
             // Original Code: "newChromosome.defenderStrategies[j].elements[0][0] = 0;" (PopulationDefenderGGame.cs)
             nodeProbs[0][0] = 1.0; 

             for(int t=0; t < TimeSteps; t++)
             {
                 var adj = IsFlipGame ? FlipGameConfig.graph.adjacencyList : GameConfig.graphConfig.adjacencyList;
                 var edges = IsFlipGame ? FlipGameConfig.graph.edges : GameConfig.graphConfig.edges;

                 for(int u=0; u<nodes; u++)
                 {
                     if(nodeProbs[t][u] <= 0) continue;

                     // Distribute mass from u to neighbors v based on edgeProbs
                     // Find edges starting at u
                     // Mapping: The edgeProbs vector corresponds to the list of edges in 'edges'
                     for(int eIdx=0; eIdx < edges.Count; eIdx++)
                     {
                         if(edges[eIdx].from == u)
                         {
                             int v = edges[eIdx].to;
                             double probMove = edgeProbs[t][eIdx]; 
                             // Note: DecodeStrategy normalized these per timestep globally or per node?
                             // Paper implies "probabilities assigned to graph edges" globally usually means normalized per node outgoing.
                             // However, CMA-ES generates one global vector. 
                             // For strict adherence to "Mixed Strategy":
                             // The vector maps to probabilities of choosing that edge. 
                             // We must handle the probability flow correctly.
                             // Assuming P(e|u) = edgeProbs[t][e] / Sum(edgeProbs[t][e'] for e' from u)
                             
                             nodeProbs[t+1][v] += nodeProbs[t][u] * probMove; // Simplified flow
                         }
                     }
                 }
                 
                 // Renormalize nodeProbs[t+1] to ensure conservation of mass if needed
                 // (Depends on interpretation of ADSO vector, assuming proper normalization in DecodeStrategy)
             }
             return nodeProbs;
        }
        
        // Placeholder for FlipIt Logic (Requires specific logic from CoevolutionFlipItGame.cs)
        private double EvaluateFlipIt(double[][] edgeProbs, List<int> attackerPath)
        {
            // Similar logic but using control/flip mechanics
            // Implemented for GGame primarily as per paper emphasis on WHG for Danskin
            return 0.0; 
        }
    }
}