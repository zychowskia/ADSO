using GeneticMultistepCoevoSG;
using GeneticMultistepSG.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepSG
{
    public class ChromosomeAttackerGGame : ChromosomeAttacker
	{
        public override ChromosomeAttacker MakeCopy(bool isBest = false)
		{
			ChromosomeAttackerGGame result = new ChromosomeAttackerGGame();
			result.fittingFunction = fittingFunction;
            result.isBest = isBest;
			result.fittingFunction = fittingFunction;
			result.fittingFunctionSecondStage = fittingFunctionSecondStage;

			result.strategy = new List<int>();
			for (int i = 0; i < strategy.Count; i++)
				result.strategy.Add(strategy[i]);

			return result;
		}


        public override void EvaluateBestDefender()
		{
			double attackerResult = 0.0, defenderResult = 0.0;
			CoevolutionGGame.Evaluate(Program.populationDefender.chromosomes[0], this, out defenderResult, out attackerResult);

			if (attackerResult == 0.0)
			{
				attackerResult = 0.0;
				defenderResult = 0.0;
			}

			fittingFunction = attackerResult;
		}

        public override void EvaluateTopNDefender()
		{
			int N = 5;
			double attackerResult = 0.0, defenderResult = 0.0;

			for (int i = 0; i < N; i++)
			{
				double attackerResultInner = 0.0, defenderResultInner = 0.0;
				CoevolutionGGame.Evaluate(Program.populationDefender.chromosomes[i], this, out defenderResultInner, out attackerResultInner);
				attackerResult += attackerResultInner;
				defenderResult += defenderResultInner;
			}

			fittingFunction = attackerResult/N;
		}

        public override void Mutate()
		{
			int firstIntervalToMute = Program.rand.Next(strategy.Count - 1) + 1;
			for (int j = firstIntervalToMute; j < strategy.Count; j++)
			{
				if (j > 0)
					strategy[j] = MoveAttackerRandomly(strategy[j - 1]);
			}
		}


		public void Mutate2()
		{
            if (strategy.Count >= (Program.gameDefinition as Ggame).rounds + 1)
				return;

			int intervalToMute = Program.rand.Next(strategy.Count - 1) + 1;

			strategy.Add(strategy[intervalToMute]);
			for (int j = strategy.Count-1; j > intervalToMute; j--)
				strategy[j] = strategy[j - 1];
		}

		public int MoveAttackerRandomly(int v)
		{
			int neighboursCount = (Program.gameDefinition as Ggame).graphConfig.adjacencyList[v].Count;

            List<int> neighbourTargets = (Program.gameDefinition as Ggame).graphConfig.adjacencyList[v].Intersect((Program.gameDefinition as Ggame).targets).ToList();

			/*if (neighbourTargets.Count > 0 && Program.rand.Next(3) == 0)
                return neighbourTargets[Program.rand.Next(neighbourTargets.Count)];
            else*/
			return (Program.gameDefinition as Ggame).graphConfig.adjacencyList[v][Program.rand.Next(neighboursCount)];
		}



	}
}
