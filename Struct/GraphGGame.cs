using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepSG.Struct
{
    public class GraphGGame
    {
        public int vertexCount;

        public List<Edge> edges;

        public List<int>[] adjacencyList;

        public void MakeAdjacencyList()
        {
            adjacencyList = new List<int>[vertexCount];
            for (int i = 0; i < vertexCount; i++)
                adjacencyList[i] = new List<int>() { i };

            foreach (Edge e in edges)
                adjacencyList[e.from].Add(e.to);
        }
    }

    public class Edge
    {
        public int from;
        public int to;
    }
}
