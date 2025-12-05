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
    public class ChromosomeAttackerFlipItGame : ChromosomeAttacker
    {
        public override ChromosomeAttacker MakeCopy(bool isBest = false)
        {
            ChromosomeAttackerFlipItGame result = new ChromosomeAttackerFlipItGame();
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
            CoevolutionFlipItGame.Evaluate(Program.populationDefender.chromosomes[0], this, out defenderResult, out attackerResult);

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
                CoevolutionFlipItGame.Evaluate(Program.populationDefender.chromosomes[i], this, out defenderResultInner, out attackerResultInner);
                attackerResult += attackerResultInner;
                defenderResult += defenderResultInner;
            }

            fittingFunction = attackerResult / N;
        }


        public override void Mutate()
        {
            int intervalToMute = Program.rand.Next((Program.gameDefinition as FlipItGame).rounds);
            strategy[intervalToMute] = MoveAttackerRandomly();
        }

        public int MoveAttackerRandomly()
        {
            List<int> possibleMoves = new List<int>() { -1 };
            for (int i = 0; i < (Program.gameDefinition as FlipItGame).graph.vertexCount; i++)
                possibleMoves.Add(i);

            return possibleMoves[Program.rand.Next(possibleMoves.Count)];
        }
    }
}
