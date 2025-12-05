using GeneticMultistepCoevoSG.Struct;
using GeneticMultistepSG;
using GeneticMultistepSG.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepCoevoSG
{
    public class ChromosomeDefenderFlipItGame : ChromosomeDefender
    {
        public override ChromosomeDefender MakeCopy()
        {
            ChromosomeDefenderFlipItGame result = new ChromosomeDefenderFlipItGame();
            result.fittingFunction = fittingFunction;
            result.fittingFunctionSecondStage = fittingFunctionSecondStage;
            result.attackerResult = attackerResult;
            if (attackStrategy == null)
                result.attackStrategy = null;
            else
                result.attackStrategy = attackStrategy.Select(x => x).ToList();

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

            fittingFunctionSecondStage = 0;

            foreach (List<int> attackerStrategy in (Program.gameDefinition as FlipItGame).attackerStrategies)
            {
                ChromosomeAttackerFlipItGame attacker = new ChromosomeAttackerFlipItGame();
                attacker.strategy = attackerStrategy;

                double defenderResult;
                CoevolutionFlipItGame.Evaluate(this, attacker, out defenderResult, out attackerResult);

                if (attackerResult > bestResultAttacker || (Math.Abs(attackerResult - bestResultAttacker) < Program.EPS && defenderResult > defenderResultForBestAttackerMove))
                {
                    bestResultAttacker = attackerResult;
                    defenderResultForBestAttackerMove = defenderResult;
                    bestAttackerStrategy = attackerStrategy;
                }
            }

            fittingFunction = defenderResultForBestAttackerMove;
            this.attackerResult = bestResultAttacker;
            attackStrategy = bestAttackerStrategy;
        }


        public override void EvaluateAttackerPopulation()
        {
            List<int> bestAttackerStrategy = new List<int>();
            double bestResultAttacker = -int.MaxValue;
            double defenderResultForBestAttackerMove = -int.MaxValue;

            fittingFunctionSecondStage = 0;

            foreach (ChromosomeAttackerFlipItGame attacker in Program.populationAttacker.chromosomes)
            {
                double defenderResult;
                CoevolutionFlipItGame.Evaluate(this, attacker, out defenderResult, out attackerResult);

                if (attackerResult > bestResultAttacker || (Math.Abs(attackerResult - bestResultAttacker) < Program.EPS && defenderResult > defenderResultForBestAttackerMove))
                {
                    bestResultAttacker = attackerResult;
                    defenderResultForBestAttackerMove = defenderResult;
                    //bestAttackerStrategy = attackerStrategy;
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

            int intervalToMute = Program.rand.Next((Program.gameDefinition as FlipItGame).rounds);
            defenderToMute.elements[strategyToMuteIndex][intervalToMute] = MoveDefenderRandomly();
        }


        public int MoveDefenderRandomly()
        {
            List<int> possibleMoves = new List<int>() { -1 };
            for (int i = 0; i < (Program.gameDefinition as FlipItGame).graph.vertexCount; i++)
                possibleMoves.Add(i);

            return possibleMoves[Program.rand.Next(possibleMoves.Count)];
        }


        public void LocalOptimization()
        {

        }

    }
}