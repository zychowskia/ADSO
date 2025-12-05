using GeneticMultistepCoevoSG.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepSG.Struct
{
    public class Ggame : Game
    {
        public GraphGGame graphConfig;

        public int rounds;

        public int defenderUnitCount;

        public List<int> targets;

        public int spawn;

        public List<double> vertexDefenderRewards;

        public List<double> vertexAttackerPenalties;

        public List<double> targetDefenderPenalties;

        public List<double> targetAttackerRewards;

        public string id;

        public List<List<int>> attackerStrategies;

		public List<bool> attackerStrategiesConsidered;

		public void ComputeAttackerStrategies()
        {
            attackerStrategies = new List<List<int>>();
			attackerStrategiesConsidered = new List<bool>();

			ExtendAttackerStrategy(new List<int>() { spawn });
        }

        private void ExtendAttackerStrategy(List<int> currentStrategy)
        {
            int v = currentStrategy.Last();

            if (targets.Contains(v) || currentStrategy.Count == rounds+1)
            {
                attackerStrategies.Add(currentStrategy.Select(x => x).ToList());
				attackerStrategiesConsidered.Add(false);

				return;
            }

            if (currentStrategy.Count > rounds)
                return;

            for (int i = 0; i < graphConfig.adjacencyList[v].Count; i++)
            {
                currentStrategy.Add(graphConfig.adjacencyList[v][i]);
                ExtendAttackerStrategy(currentStrategy);
                currentStrategy.RemoveAt(currentStrategy.Count - 1);
            }

        }
    }
}
