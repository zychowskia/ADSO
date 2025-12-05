using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepCoevoSG
{
    public abstract class Population
    {
        public int populationSize = 50;

        public int elite = 2;

        public double mutationRate = 0.8;

        public double mutationRepeats = 2;

        public double crossoverRate = 0.7;

        public double selectionPressure = 0.8;

        public bool isLocalOptimization = false;

        public int crossoverVersion = 0;

        public int evaluationVersion = 0;
    }
}
