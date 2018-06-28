using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicTopoSort
{
    class Program
    {
        public static IEnumerable<IEnumerable<int>> SortPk(int nVertices, IEnumerable<(int, int)> edges)
        {
            var nodes = PkAlgorithm.InitGraph(nVertices);
            var graph = nodes.ToList();
            foreach (var edge in edges)
            {
                PkAlgorithm.AddEdge(graph, nodes[edge.Item1], nodes[edge.Item2]);
                yield return graph.Select(n => n.Number);
            }
        }

        public static IEnumerable<IEnumerable<int>> SortBucket(int nVertices, IEnumerable<(int, int)> edges)
        {
            var graph = BucketAlgorithm.InitGraph(nVertices);
            foreach (var edge in edges)
            {
                BucketAlgorithm.AddEdge(graph, graph.Nodes[edge.Item1], graph.Nodes[edge.Item2]);
                yield return graph.SortedList.OfType<BucketAlgorithm.Node>().Select(n => n.Index);
            }
        }

        static IEnumerable<(int, int)> ReadInput(int nEdges)
        {
            for (int i = 0; i < nEdges; i++)
            {
                var edge = Console.ReadLine().Split().Select(int.Parse).ToArray();
                yield return (edge[0], edge[1]);
            }
        }

        static void Main(string[] args)
        {
            var nm = Console.ReadLine().Split().Select(int.Parse).ToArray();
            foreach (var l in SortBucket(nm[0], ReadInput(nm[1])))
            {
                foreach (var n in l)
                    Console.Write(n + " ");
                Console.WriteLine();
            }
        }
    }
}
