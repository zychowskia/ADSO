using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMultistepCoevoSG.Struct
{
    public class GraphFlipItGame
    {
        public int vertexCount;

        public List<Edge> edges;

        public List<int>[] adjacencyList;

        public List<int>[] predecessorList;

        public void MakeAdjacencyList()
        {
            adjacencyList = new List<int>[vertexCount];
            for (int i = 0; i < vertexCount; i++)
                adjacencyList[i] = new List<int>();

            predecessorList = new List<int>[vertexCount];
            for (int i = 0; i < vertexCount; i++)
                predecessorList[i] = new List<int>();

            foreach (Edge e in edges)
            {
                adjacencyList[e.from].Add(e.to);
                predecessorList[e.to].Add(e.from);
            }
        }

    }

    public class Edge
    {
        public int from;
        public int to;

        public Edge() { }

        public Edge(int _from, int _to)
        {
            from = _from;
            to = _to;
        }
    }
}
