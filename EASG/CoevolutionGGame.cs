using GeneticMultistepCoevoSG;
using GeneticMultistepSG.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepSG
{
	public class CoevolutionGGame
	{
		public static void Evaluate(ChromosomeDefender defender, ChromosomeAttacker attacker, out double defenderResult, out double attackerResult)
		{
			attackerResult = 0.0; defenderResult = 0.0;
			double currentProbablility = 1.0;

			for (int interval = 0; interval < attacker.strategy.Count; interval++)
			{
				int v = attacker.strategy[interval];

				double probabilityAttackerCaught = 0.0;
				for (int j = 0; j < defender.defenderStrategies[0].elements.Count; j++)
				{
					bool isCaught = false;
					for (int i = 0; i < defender.defenderStrategies.Length; i++) 
					{
						if (defender.defenderStrategies[i].elements[j][interval] == v)
							isCaught = true;
					}
					if (isCaught)
						probabilityAttackerCaught += defender.defenderStrategies[0].probabilities[j];
				}

				double defenderReward = (Program.gameDefinition as Ggame).vertexDefenderRewards[v];
                double attackerPenality = (Program.gameDefinition as Ggame).vertexAttackerPenalties[v];

				defenderResult += currentProbablility * probabilityAttackerCaught * defenderReward;
				attackerResult += currentProbablility * probabilityAttackerCaught * attackerPenality;

				if ((Program.gameDefinition as Ggame).targets.Contains(v)) //znajdujemy się w celu
				{
					double defenderPanality = (Program.gameDefinition as Ggame).targetDefenderPenalties[(Program.gameDefinition as Ggame).targets.IndexOf(v)];
                    double attackerReward = (Program.gameDefinition as Ggame).targetAttackerRewards[(Program.gameDefinition as Ggame).targets.IndexOf(v)];

					defenderResult += currentProbablility * (1 - probabilityAttackerCaught) * defenderPanality;
					attackerResult += currentProbablility * (1 - probabilityAttackerCaught) * attackerReward;

					return;
				}

				currentProbablility = currentProbablility * (1 - probabilityAttackerCaught);

			}
		}

	}
}
