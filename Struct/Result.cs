using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepSG.Struct
{
    public class Result
    {
        public double trainingTime;

        public Payoff payoff;
    }

    public class Payoff
    {
        public double defender;

        public double attacker;
    }
}
