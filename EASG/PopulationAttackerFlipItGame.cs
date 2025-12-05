using GeneticMultistepCoevoSG.Struct;
using GeneticMultistepSG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepCoevoSG
{
    public class PopulationAttackerFlipItGame : PopulationAttacker
    {
   public static int theSameCrossover = 0;
        public static int allCrossover = 0;

        public static int itAll = 0;


        public PopulationAttackerFlipItGame()
        {
            chromosomes = new List<ChromosomeAttacker>();
        }


        public override void InitPopulation()
        {
            itAll = 0;

            chromosomes = new List<ChromosomeAttacker>();
            Random rand = new Random();
            for (int i = 0; i < populationSize; i++)
            {
                ChromosomeAttackerFlipItGame newChromosome = new ChromosomeAttackerFlipItGame();
                newChromosome.strategy = (Program.gameDefinition as FlipItGame).attackerStrategies[rand.Next((Program.gameDefinition as FlipItGame).attackerStrategies.Count)];

                chromosomes.Add(newChromosome);
            }
        }



        public ChromosomeAttackerFlipItGame Crossover(ChromosomeAttackerFlipItGame c1, ChromosomeAttackerFlipItGame c2)
        {
            ChromosomeAttackerFlipItGame result = c1.MakeCopy() as ChromosomeAttackerFlipItGame;

            for (int i = 0; i < c1.strategy.Count / 2 - 1; i++)
                result.strategy[i] = c2.strategy[i];

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
                listAfterCrossover.Add(Crossover(listToCrossover[i] as ChromosomeAttackerFlipItGame, listToCrossover[i + 1] as ChromosomeAttackerFlipItGame));

            chromosomes = listAfterCrossover;


            for (int i = 0; i < chromosomes.Count; i++)
                if (Program.rand.NextDouble() < mutationRate)
                    chromosomes[i].Mutate();


            foreach (ChromosomeAttackerFlipItGame c in chromosomes)
                if (evaluationVersion == 0)
                    c.EvaluateBestDefender();
                else
                    c.EvaluateTopNDefender();
            foreach (ChromosomeAttackerFlipItGame c in newChromosomes)
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
            }

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
