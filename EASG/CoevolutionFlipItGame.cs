using GeneticMultistepCoevoSG.Struct;
using GeneticMultistepSG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepCoevoSG
{
    public class CoevolutionFlipItGame
    {
        public static void Evaluate(ChromosomeDefender defender, ChromosomeAttacker attacker, out double defenderResult, out double attackerResult)
        {
            attackerResult = 0.0;
            defenderResult = 0.0;

            List<double> isVertexControlledByAttacker = new List<double>(); 
            for (int i = 0; i < (Program.gameDefinition as FlipItGame).graph.vertexCount; i++)
                isVertexControlledByAttacker.Add(0.0); 

            for (int interval = 0; interval < attacker.strategy.Count; interval++)
            {
                int vAttack = attacker.strategy[interval]; 
                double probabilityAttackerFlipPossible = 0.0; 
                if ((Program.gameDefinition as FlipItGame).entryNodes.Contains(vAttack) || vAttack == -1)
                    probabilityAttackerFlipPossible = 1.0;
                else
                {
                    foreach (int predecessor in (Program.gameDefinition as FlipItGame).graph.predecessorList[vAttack])
                        probabilityAttackerFlipPossible = Math.Max(probabilityAttackerFlipPossible, isVertexControlledByAttacker[predecessor]);
                }


                List<double> probabilityDefenderFlip = new List<double>();
                List<double> probabilityDefenderFlipPossible = new List<double>();
                for (int v = 0; v < (Program.gameDefinition as FlipItGame).graph.vertexCount; v++)
                {
                    probabilityDefenderFlip.Add(0.0);
                    probabilityDefenderFlipPossible.Add(0.0);

                    if ((Program.gameDefinition as FlipItGame).entryNodes.Contains(v))
                        probabilityDefenderFlipPossible[v] = 1.0;
                    else
                    {
                        foreach (int predecessor in (Program.gameDefinition as FlipItGame).graph.predecessorList[v])
                            probabilityDefenderFlipPossible[v] = Math.Max(probabilityDefenderFlipPossible[v], 1 - isVertexControlledByAttacker[predecessor]);
                    }

                }

                for (int v = 0; v < (Program.gameDefinition as FlipItGame).graph.vertexCount; v++)
                {
                    for (int j = 0; j < defender.defenderStrategies[0].elements.Count; j++) 
                    {
                        bool isCaught = false;
                        for (int i = 0; i < defender.defenderStrategies.Length; i++) 
                        {
                            if (defender.defenderStrategies[i].elements[j][interval] == v)
                                isCaught = true;
                        }
                        if (isCaught)
                            probabilityDefenderFlip[v] += defender.defenderStrategies[0].probabilities[j];
                    }
                }

                for (int v = 0; v < isVertexControlledByAttacker.Count; v++)
                {
                    if (v == vAttack)
                    {
                        double probAttackSuccessful = 0.0;
                        probAttackSuccessful = probabilityAttackerFlipPossible
                            * (isVertexControlledByAttacker[v] 
                            + (1 - isVertexControlledByAttacker[v]) * (1 - probabilityDefenderFlip[v]));

                        isVertexControlledByAttacker[v] = probAttackSuccessful;
                    }
                    else
                    {
                        isVertexControlledByAttacker[v] = isVertexControlledByAttacker[v] * (1 - probabilityDefenderFlip[v] * probabilityDefenderFlipPossible[v]);
                    }
                }

                if (vAttack != -1)
                    attackerResult += (-(Program.gameDefinition as FlipItGame).attackerCosts[vAttack]);
                for (int v = 0; v < isVertexControlledByAttacker.Count; v++)
                    attackerResult += isVertexControlledByAttacker[v] * (Program.gameDefinition as FlipItGame).attackerRewards[v];

                for (int v = 0; v < probabilityDefenderFlip.Count; v++)
                    defenderResult += (-(Program.gameDefinition as FlipItGame).defenderCosts[v]) * probabilityDefenderFlip[v];
                for (int v = 0; v < isVertexControlledByAttacker.Count; v++)
                    defenderResult += (1 - isVertexControlledByAttacker[v]) * (Program.gameDefinition as FlipItGame).defenderRewards[v];

            }



        }
    }
}
