using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepSG.Struct
{
    public class DefenderStrategy
    {
        public List<int[]> elements;

        public List<double> probabilities;

        public DefenderStrategy()
        {
            elements = new List<int[]>();
            probabilities = new List<double>();
        }

        public DefenderStrategy Copy(List<int> toRemove = null)
        {
            DefenderStrategy result = new DefenderStrategy();
            for (int i = 0; i < elements.Count; i++)
            {
                if (toRemove != null && toRemove.Contains(i))
                    continue;
                result.elements.Add(new int[elements[i].Length]);
                for (int j = 0; j < elements[i].Length; j++)
                    result.elements.Last()[j] = elements[i][j];

                result.probabilities.Add(probabilities[i]);
            }

            double probabilitiesSum = result.probabilities.Sum();
            result.probabilities = result.probabilities.Select(x => x * (1 / probabilitiesSum)).ToList();

            return result;
        }
    }
}
