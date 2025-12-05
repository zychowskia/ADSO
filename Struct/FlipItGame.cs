using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepCoevoSG.Struct
{
    public class FlipItGame : Game
    {
        public GraphFlipItGame graph;

        public int rounds;

        public List<int> entryNodes;

        public int defenderUnitCount = 1;

        public List<double> attackerRewards;

        public List<double> defenderRewards;

        public List<double> attackerCosts;

        public List<double> defenderCosts;

        public string id;

        public List<List<int>> attackerStrategies;

        public void ComputeAttackerStrategies()
        {
            attackerStrategies = new List<List<int>>();
            ExtendAttackerStrategy(new List<int>());
        }

        private void ExtendAttackerStrategy(List<int> currentStrategy)
        {
            if (currentStrategy.Count == rounds)
            {
                attackerStrategies.Add(currentStrategy.Select(x => x).ToList());
                return;
            }

            HashSet<int> consideredVertices = new HashSet<int>();
            consideredVertices.Add(-1);
            for (int i = 0; i < entryNodes.Count; i++)
                if (!consideredVertices.Contains(entryNodes[i]))
                    consideredVertices.Add(entryNodes[i]);

            for (int i = 0; i < currentStrategy.Count; i++)
            {
                if (!consideredVertices.Contains(currentStrategy[i]))
                    consideredVertices.Add(currentStrategy[i]);

                if (currentStrategy[i] == -1) continue;

                for (int j = 0; j < graph.adjacencyList[currentStrategy[i]].Count; j++)
                {
                    if (!consideredVertices.Contains(graph.adjacencyList[currentStrategy[i]][j]))
                        consideredVertices.Add(graph.adjacencyList[currentStrategy[i]][j]);
                }
            }

            foreach (int v in consideredVertices)
            {
                currentStrategy.Add(v);
                ExtendAttackerStrategy(currentStrategy);
                currentStrategy.RemoveAt(currentStrategy.Count - 1);
            }
        }
    }
}
