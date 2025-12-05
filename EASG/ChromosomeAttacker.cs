using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepCoevoSG
{
    public abstract class ChromosomeAttacker : Chromosome
    {
        public List<int> strategy;

        public double fittingFunction;

        public double fittingFunctionSecondStage;

        public bool isBest;


        public abstract ChromosomeAttacker MakeCopy(bool isBest = false);

        public abstract void Mutate();

        public abstract void EvaluateBestDefender();

        public abstract void EvaluateTopNDefender();
    }
}
