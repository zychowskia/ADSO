using GeneticMultistepCoevoSG;
using GeneticMultistepSG.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepSG
{
    public class PopulationAttackerGGame : PopulationAttacker
    {
        public static int theSameCrossover = 0;
        public static int allCrossover = 0;

        public static int itAll = 0;
        public static int howManyTheSamePairs = 0;
        public static int howManyTheSameMax = 0;
        public static int howManyDistinct = 0;




        public PopulationAttackerGGame()
        {
            chromosomes = new List<ChromosomeAttacker>();
        }


        public override void InitPopulation()
        {
            itAll = 0;
            howManyTheSameMax = 0;
            howManyTheSamePairs = 0;
            howManyDistinct = 0;

            chromosomes = new List<ChromosomeAttacker>();
            Random rand = new Random();
            for (int i = 0; i < populationSize; i++)
            {
                ChromosomeAttackerGGame newChromosome = new ChromosomeAttackerGGame();
                newChromosome.strategy = (Program.gameDefinition as Ggame).attackerStrategies[rand.Next((Program.gameDefinition as Ggame).attackerStrategies.Count)];

                chromosomes.Add(newChromosome);
            }
        }


        public ChromosomeAttackerGGame Crossover(ChromosomeAttackerGGame c1, ChromosomeAttackerGGame c2)
        {
            ChromosomeAttackerGGame result = c1.MakeCopy() as ChromosomeAttackerGGame;
            for (int i = c2.strategy.Count; i < c1.strategy.Count; i++)
                c2.strategy.Add(c1.strategy[i]);

            int length = c1.strategy.Count / 2;
            for (int i = 0; i < c1.strategy.Count / 2 - 1; i++)
            {
                if (length + i + 1 < c2.strategy.Count)
                    if (c1.strategy[length + i] == c2.strategy[length + i]
                        || (Program.gameDefinition as Ggame).graphConfig.adjacencyList[c1.strategy[length + i]].Contains(c2.strategy[length + i + 1]))
                    {
                        for (int j = length + i + 1; j < c1.strategy.Count; j++)
                            result.strategy[j] = c2.strategy[j];
                        break;
                    }

                if (length - i + 1 < c2.strategy.Count)
                    if (c1.strategy[length - i] == c2.strategy[length - i]
                    || (Program.gameDefinition as Ggame).graphConfig.adjacencyList[c1.strategy[length - i]].Contains(c2.strategy[length - i + 1]))
                    {
                        for (int j = length - i + 1; j < c1.strategy.Count; j++)
                            result.strategy[j] = c2.strategy[j];
                        break;
                    }
            }

            bool isTheSame1 = true, isTheSame2 = true;
            if (string.Join("#", c1.strategy) != string.Join("#", result.strategy))
                isTheSame1 = false;
            if (string.Join("#", c2.strategy) != string.Join("#", result.strategy))
                isTheSame2 = false;

            if (isTheSame1 || isTheSame2)
                theSameCrossover++;

            allCrossover++;

            return result;
        }


        public override void MakeNewPopulation()
        {
            List<ChromosomeAttacker> newChromosomes = new List<ChromosomeAttacker>();

            for (int i = 0; i < elite; i++)
            {
                newChromosomes.Add(chromosomes[i].MakeCopy());
                if (evaluationVersion == 0)
                    newChromosomes.Last().EvaluateBestDefender();
                else
                    newChromosomes.Last().EvaluateTopNDefender();
            }

            newChromosomes.AddRange(chromosomes.Where(x => x.isBest).Select(x => x.MakeCopy(true)));


            List<ChromosomeAttacker> listToCrossover = new List<ChromosomeAttacker>();
            List<ChromosomeAttacker> listAfterCrossover = new List<ChromosomeAttacker>();

            for (int i = 0; i < chromosomes.Count; i++)
            {
                if (Program.rand.NextDouble() < crossoverRate)
                    listToCrossover.Add(chromosomes[i]);
                listAfterCrossover.Add(chromosomes[i]);
            }

            listToCrossover = PermutateElements(listToCrossover);

            for (int i = 0; i < listToCrossover.Count - 1; i += 2)
                listAfterCrossover.Add(Crossover(listToCrossover[i] as ChromosomeAttackerGGame, listToCrossover[i + 1] as ChromosomeAttackerGGame));

            chromosomes = listAfterCrossover;

            for (int i = 0; i < chromosomes.Count; i++)
                if (Program.rand.NextDouble() < mutationRate)
                    chromosomes[i].Mutate();

            foreach (ChromosomeAttackerGGame c in chromosomes)
            {
                int targetIndex = c.strategy.FindIndex(x => (Program.gameDefinition as Ggame).targets.Contains(x));
                if (targetIndex > 0)
                    c.strategy.RemoveRange(targetIndex + 1, c.strategy.Count - targetIndex - 1);
            }

            foreach (ChromosomeAttackerGGame c in chromosomes)
                if (evaluationVersion == 0)
                    c.EvaluateBestDefender();
                else
                    c.EvaluateTopNDefender();
            foreach (ChromosomeAttackerGGame c in newChromosomes)
                if (evaluationVersion == 0)
                    c.EvaluateBestDefender();
                else
                    c.EvaluateTopNDefender();


            while (newChromosomes.Count < populationSize)
            {
                int c1 = Program.rand.Next(chromosomes.Count);
                int c2 = Program.rand.Next(chromosomes.Count);

                double k = Program.rand.NextDouble();
                if ((chromosomes[c1].fittingFunction > chromosomes[c2].fittingFunction
                    || (chromosomes[c1].fittingFunction == chromosomes[c2].fittingFunction && chromosomes[c1].fittingFunctionSecondStage > chromosomes[c2].fittingFunctionSecondStage))
                    && k < selectionPressure)
                    newChromosomes.Add(chromosomes[c1].MakeCopy());
                else
                    newChromosomes.Add(chromosomes[c2].MakeCopy());
            }


            chromosomes = newChromosomes;

            chromosomes.Sort((c1, c2) => (c2.fittingFunction != c1.fittingFunction ? c2.fittingFunction.CompareTo(c1.fittingFunction) : string.Join(",", c2.strategy).CompareTo(string.Join(",", c1.strategy))));

            /*for (int i = 0; i < 5; i++)
                Console.Write(chromosomes[i].fittingFunction + " ");
            Console.WriteLine();*/

            /*foreach (ChromosomeAttacker c in chromosomes)
            {
                int i = Program.gameDefinition.attackerStrategies.FindIndex(x => string.Join(",", x) == string.Join(",", c.strategy));
                if (i >= 0)
                    Program.gameDefinition.attackerStrategiesConsidered[i] = true;
            }*/

            itAll++;

            int bestPairs = 0;
            for (int i = 0; i < newChromosomes.Count; i++)
            {
                int pairs = 0;
                for (int j = i + 1; j < newChromosomes.Count; j++)
                    if (string.Join("#", newChromosomes[j].strategy) == string.Join("#", newChromosomes[i].strategy))
                        pairs++;

                if (pairs > bestPairs)
                    bestPairs = pairs;
                howManyTheSamePairs += pairs;
            }

            howManyTheSameMax += bestPairs;

            howManyDistinct += newChromosomes.Select(x => string.Join("#", x.strategy)).Distinct().Count();

        }

        public List<ChromosomeAttacker> PermutateElements(List<ChromosomeAttacker> list)
        {
            List<ChromosomeAttacker> randomizedList = new List<ChromosomeAttacker>();
            while (list.Count > 0)
            {
                int index = Program.rand.Next(0, list.Count);
                randomizedList.Add(list[index]);
                list.RemoveAt(index);
            }

            return randomizedList;
        }

    }
}
