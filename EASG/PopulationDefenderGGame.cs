using GeneticMultistepCoevoSG;
using GeneticMultistepSG.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepSG
{
    public class PopulationDefenderGGame : PopulationDefender
    {

        public PopulationDefenderGGame()
        {
            chromosomes = new List<ChromosomeDefender>();
        }


        public override void InitPopulation()
        {
            chromosomes = new List<ChromosomeDefender>();
            for (int i = 0; i < populationSize; i++)
            {
                ChromosomeDefenderGGame newChromosome = new ChromosomeDefenderGGame();
                newChromosome.defenderStrategies = new DefenderStrategy[(Program.gameDefinition as Ggame).defenderUnitCount];
                for (int j = 0; j < newChromosome.defenderStrategies.Length; j++)
                {
                    newChromosome.defenderStrategies[j] = new DefenderStrategy();
                    newChromosome.defenderStrategies[j].elements.Add(new int[(Program.gameDefinition as Ggame).rounds + 1]);

                    newChromosome.defenderStrategies[j].elements[0][0] = 0;

                    for (int k = 1; k < newChromosome.defenderStrategies[j].elements[0].Length; k++)
                    {
                        int prevPosition = newChromosome.defenderStrategies[j].elements[0][k - 1];
                        newChromosome.defenderStrategies[j].elements[0][k] = newChromosome.MoveDefenderRandomly(newChromosome.defenderStrategies[j].elements[0][k - 1]);
                    }

                    newChromosome.defenderStrategies[j].probabilities.Add(1.0);
                }

                chromosomes.Add(newChromosome);
            }
        }



        public ChromosomeDefenderGGame Crossover(ChromosomeDefenderGGame c1, ChromosomeDefenderGGame c2)
        {
            List<double> randomNumbers = new List<double>();
            for (int i = 0; i < c1.defenderStrategies[0].probabilities.Count + c2.defenderStrategies[0].probabilities.Count; i++)
                randomNumbers.Add(Program.rand.NextDouble() * Program.rand.NextDouble());

            ChromosomeDefenderGGame result = new ChromosomeDefenderGGame();
            result.defenderStrategies = new DefenderStrategy[c1.defenderStrategies.Length];
            for (int i = 0; i < c1.defenderStrategies.Length; i++)
            {
                result.defenderStrategies[i] = new DefenderStrategy();
                DefenderStrategy c1Strategy = c1.defenderStrategies[i].Copy();
                for (int j = 0; j < c1Strategy.elements.Count; j++)
                {
                    result.defenderStrategies[i].elements.Add(c1Strategy.elements[j]);
                    result.defenderStrategies[i].probabilities.Add(c1Strategy.probabilities[j] / 2);
                }
                DefenderStrategy c2Strategy = c2.defenderStrategies[i].Copy();
                for (int j = 0; j < c2Strategy.elements.Count; j++)
                {
                    result.defenderStrategies[i].elements.Add(c2Strategy.elements[j]);
                    result.defenderStrategies[i].probabilities.Add(c2Strategy.probabilities[j] / 2);
                }

                List<int> strategiesToRemove = new List<int>();
                for (int j = 0; j < result.defenderStrategies[i].probabilities.Count; j++)
                    if (randomNumbers[j] > result.defenderStrategies[i].probabilities[j] && result.defenderStrategies[i].probabilities[j] < result.defenderStrategies[i].probabilities.Max()) //maksymalnej nie usuwamy
                        strategiesToRemove.Add(j);

                result.defenderStrategies[i] = result.defenderStrategies[i].Copy(strategiesToRemove);
            }

            return result;
        }

        public ChromosomeDefenderGGame Crossover2(ChromosomeDefenderGGame c1, ChromosomeDefenderGGame c2)
        {
            List<double> randomNumbers = new List<double>();
            bool isOneLessThan05 = false;

            while (!isOneLessThan05)
            {
                randomNumbers = new List<double>();
                for (int i = 0; i < c1.defenderStrategies[0].probabilities.Count + c2.defenderStrategies[0].probabilities.Count; i++)
                {
                    randomNumbers.Add(Program.rand.NextDouble());
                    if (randomNumbers.Last() < 0.5)
                        isOneLessThan05 = true;
                }
            }

            ChromosomeDefenderGGame result = new ChromosomeDefenderGGame();
            result.defenderStrategies = new DefenderStrategy[c1.defenderStrategies.Length];
            for (int i = 0; i < c1.defenderStrategies.Length; i++)
            {
                result.defenderStrategies[i] = new DefenderStrategy();
                DefenderStrategy c1Strategy = c1.defenderStrategies[i].Copy();
                for (int j = 0; j < c1Strategy.elements.Count; j++)
                {
                    result.defenderStrategies[i].elements.Add(c1Strategy.elements[j]);
                    result.defenderStrategies[i].probabilities.Add(c1Strategy.probabilities[j] / 2);
                }
                DefenderStrategy c2Strategy = c2.defenderStrategies[i].Copy();
                for (int j = 0; j < c2Strategy.elements.Count; j++)
                {
                    result.defenderStrategies[i].elements.Add(c2Strategy.elements[j]);
                    result.defenderStrategies[i].probabilities.Add(c2Strategy.probabilities[j] / 2);
                }

                List<int> strategiesToRemove = new List<int>();

                strategiesToRemove = new List<int>();
                for (int j = 0; j < result.defenderStrategies[i].probabilities.Count; j++)
                    if (randomNumbers[j] > 0.5)
                        strategiesToRemove.Add(j);

                result.defenderStrategies[i] = result.defenderStrategies[i].Copy(strategiesToRemove);
            }

            return result;
        }


        public override void MakeNewPopulation()
        {
            List<ChromosomeDefender> newChromosomes = new List<ChromosomeDefender>();

            for (int i = 0; i < elite; i++)
            {
                newChromosomes.Add(chromosomes[i].MakeCopy());
                newChromosomes.Last().EvaluateAttackerPopulation();
				//newChromosomes.Last().EvaluateOptimal();
			}

            List<ChromosomeDefender> listToCrossover = new List<ChromosomeDefender>(); 
            List<ChromosomeDefender> listAfterCrossover = new List<ChromosomeDefender>(); 

            for (int i = 0; i < chromosomes.Count; i++)
            {
                if (Program.rand.NextDouble() < crossoverRate)
                    listToCrossover.Add(chromosomes[i]);
                listAfterCrossover.Add(chromosomes[i]);
            }

            listToCrossover = PermutateElements(listToCrossover);

            for (int i = 0; i < listToCrossover.Count - 1; i += 2)
                if (crossoverVersion == 0)
                    listAfterCrossover.Add(Crossover(listToCrossover[i] as ChromosomeDefenderGGame, listToCrossover[i + 1] as ChromosomeDefenderGGame));
                else
                    listAfterCrossover.Add(Crossover2(listToCrossover[i] as ChromosomeDefenderGGame, listToCrossover[i + 1] as ChromosomeDefenderGGame));

            chromosomes = listAfterCrossover;


            for (int i = 0; i < chromosomes.Count; i++)
                if (Program.rand.NextDouble() < mutationRate)
                    for (int j = 0; j < mutationRepeats; j++)
                        chromosomes[i].Mutate();

            if (isLocalOptimization)
                foreach (ChromosomeDefenderGGame c in chromosomes)
                    c.LocalOptimization();

			foreach (ChromosomeDefenderGGame c in chromosomes)
			{
				c.EvaluateAttackerPopulation();
				//c.EvaluateOptimal();
			}


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

            chromosomes.Sort((c1, c2) => (c2.fittingFunction != c1.fittingFunction ? c2.fittingFunction.CompareTo(c1.fittingFunction) : c2.fittingFunctionSecondStage.CompareTo(c1.fittingFunctionSecondStage)));

            /*for (int i = 0; i < 5; i++)
				Console.Write(chromosomes[i].fittingFunction + " ");
			Console.WriteLine();*/

        }


        public List<ChromosomeDefender> PermutateElements(List<ChromosomeDefender> list)
        {
            List<ChromosomeDefender> randomizedList = new List<ChromosomeDefender>();
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
