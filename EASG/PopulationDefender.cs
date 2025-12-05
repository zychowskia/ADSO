using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepCoevoSG
{
    public abstract class PopulationDefender : Population
    {
        public List<ChromosomeDefender> chromosomes;

        public abstract void InitPopulation();

        public abstract void MakeNewPopulation();
    }
}
