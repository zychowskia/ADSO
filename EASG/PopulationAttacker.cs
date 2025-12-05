using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepCoevoSG
{
    public abstract class PopulationAttacker : Population
    {
        public List<ChromosomeAttacker> chromosomes;

        public abstract void InitPopulation();

        public abstract void MakeNewPopulation();
    }
}
