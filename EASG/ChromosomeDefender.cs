using GeneticMultistepSG.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepCoevoSG
{
    public abstract class ChromosomeDefender : Chromosome
    {
        public DefenderStrategy[] defenderStrategies;

        public double fittingFunction;

        public double fittingFunctionOptimal;

        public double fittingFunctionSecondStage;

        public double attackerResult;

        public double attackerResultOptimal;

        public List<int> attackStrategy;

        public List<int> attackStrategyOptimal;

        public abstract void Mutate();

        public abstract ChromosomeDefender MakeCopy();

        public abstract void EvaluateAttackerPopulation();

        public abstract void EvaluateOptimal();
    }
}
