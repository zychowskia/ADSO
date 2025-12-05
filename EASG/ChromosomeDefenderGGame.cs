using GeneticMultistepCoevoSG;
using GeneticMultistepSG.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepSG
{
    public class ChromosomeDefenderGGame : ChromosomeDefender
	{

        public override ChromosomeDefender MakeCopy()
		{
			ChromosomeDefenderGGame result = new ChromosomeDefenderGGame();
			result.fittingFunction = fittingFunction;
			result.fittingFunctionSecondStage = fittingFunctionSecondStage;
			result.attackerResult = attackerResult;
			result.fittingFunctionOptimal = fittingFunctionOptimal;
			result.attackerResultOptimal = attackerResultOptimal;
			result.attackStrategyOptimal = attackStrategyOptimal;

			if (attackStrategy == null)
				result.attackStrategy = null;
			else
				result.attackStrategy = attackStrategy.Select(x => x).ToList();

			if (attackStrategyOptimal == null)
				result.attackStrategyOptimal = null;
			else
				result.attackStrategyOptimal = attackStrategyOptimal.Select(x => x).ToList();

			result.defenderStrategies = new DefenderStrategy[defenderStrategies.Length];
			for (int i = 0; i < defenderStrategies.Length; i++)
				result.defenderStrategies[i] = defenderStrategies[i].Copy();

			return result;
		}

        public override void EvaluateOptimal()
		{
			List<int> bestAttackerStrategy = new List<int>();
			double bestResultAttacker = -int.MaxValue;
			double defenderResultForBestAttackerMove = -int.MaxValue;

            foreach (List<int> attackerStrategy in (Program.gameDefinition as Ggame).attackerStrategies)
			{
				double attackerResult = 0.0, defenderResult = 0.0;

				ChromosomeAttackerGGame attacker = new ChromosomeAttackerGGame();
				attacker.strategy = attackerStrategy;

				CoevolutionGGame.Evaluate(this, attacker, out defenderResult, out attackerResult);

                Program.attackerWriter.WriteLine(Program.currentGame + ";" + Program.currentIt + ";" + attackerResult + ";");

				if (attackerResult == 0.0)
				{
					attackerResult = 0.0;
					defenderResult = 0.0;
				}

				if (attackerResult > bestResultAttacker + Program.EPS || (Math.Abs(attackerResult - bestResultAttacker) < Program.EPS && defenderResult > defenderResultForBestAttackerMove))
				{
					bestResultAttacker = attackerResult;
					defenderResultForBestAttackerMove = defenderResult;
					bestAttackerStrategy = attackerStrategy;
				}
			}
            Program.attackerWriter.Flush();

			fittingFunctionOptimal = defenderResultForBestAttackerMove;

			if (bestResultAttacker == 0)
				attackerResultOptimal = bestResultAttacker;
			attackerResultOptimal = bestResultAttacker;
			attackStrategyOptimal = bestAttackerStrategy;
		}


        public override void EvaluateAttackerPopulation()
		{
			List<int> bestAttackerStrategy = new List<int>();
			double bestResultAttacker = -int.MaxValue;
			double defenderResultForBestAttackerMove = -int.MaxValue;
			fittingFunctionSecondStage = 0;

			foreach (ChromosomeAttackerGGame attacker in Program.populationAttacker.chromosomes)
			{
				double attackerResult = 0.0, defenderResult = 0.0;

				CoevolutionGGame.Evaluate(this, attacker, out defenderResult, out attackerResult);

				if (attackerResult == 0.0)
				{
					attackerResult = 0.0;
					defenderResult = 0.0;
				}

				if (attackerResult > bestResultAttacker + Program.EPS || (Math.Abs(attackerResult - bestResultAttacker) < Program.EPS && defenderResult > defenderResultForBestAttackerMove))
				{
					bestResultAttacker = attackerResult;
					defenderResultForBestAttackerMove = defenderResult;
					bestAttackerStrategy = attacker.strategy;
				}
			}

			fittingFunction = defenderResultForBestAttackerMove;
			this.attackerResult = bestResultAttacker;
			attackStrategy = bestAttackerStrategy;
		}


        public override void Mutate()
		{
			DefenderStrategy defenderToMute = defenderStrategies[Program.rand.Next(defenderStrategies.Length)];
			int strategyToMuteIndex = Program.rand.Next(defenderToMute.elements.Count);

			int firstIntervalToMute = Program.rand.Next(defenderToMute.elements[strategyToMuteIndex].Length - 1) + 1;
			for (int j = firstIntervalToMute; j < defenderToMute.elements[strategyToMuteIndex].Length; j++)
			{
				if (j > 0)
					defenderToMute.elements[strategyToMuteIndex][j] = MoveDefenderRandomly(defenderToMute.elements[strategyToMuteIndex][j - 1]);
			}
		}


		public int MoveDefenderRandomly(int v)
		{
			int neighboursCount = (Program.gameDefinition as Ggame).graphConfig.adjacencyList[v].Count;

            List<int> neighbourTargets = (Program.gameDefinition as Ggame).graphConfig.adjacencyList[v].Intersect((Program.gameDefinition as Ggame).targets).ToList();

			/*if (neighbourTargets.Count > 0 && Program.rand.Next(3) == 0)
                return neighbourTargets[Program.rand.Next(neighbourTargets.Count)];
            else*/
			return (Program.gameDefinition as Ggame).graphConfig.adjacencyList[v][Program.rand.Next(neighboursCount)];
		}


		public void LocalOptimization()
		{
			DefenderStrategy defenderToMute = defenderStrategies[Program.rand.Next(defenderStrategies.Length)];
			int strategyToMuteIndex = Program.rand.Next(defenderToMute.elements.Count);

			for (int j = 1; j < defenderToMute.elements[strategyToMuteIndex].Length - 1; j++)
			{
				int vPrev = defenderToMute.elements[strategyToMuteIndex][j - 1];
				int vNext = defenderToMute.elements[strategyToMuteIndex][j - 1];

                List<int> targetsToConsider = (Program.gameDefinition as Ggame).graphConfig.adjacencyList[vPrev].Intersect((Program.gameDefinition as Ggame).targets)
                    .Where(x => (Program.gameDefinition as Ggame).graphConfig.adjacencyList[x].Contains(vNext)).ToList();
				if (targetsToConsider.Count > 0)
					defenderToMute.elements[strategyToMuteIndex][j] = targetsToConsider[Program.rand.Next(targetsToConsider.Count)];
			}
		}



	}
}
