using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicTopoSort
{
    public static class PKAlgorithm
    {
        public class Node
        {
            public readonly IList<Node> Outgoing = new List<Node>();
            public readonly IList<Node> Incoming = new List<Node>();
            public int Index;
        }

        private static IEnumerable<int> Merge(IList<Node> a, IList<Node> b) 
        {
            int f = 0, s = 0;
            while (f < a.Count && s < b.Count) 
            {
                if (a[f].Index < b[s].Index) 
                {
                    yield return a[f].Index;
                    f++;
                } 
                else 
                {
                    yield return b[s].Index;
                    s++;
                }
            }
            for (; f < a.Count; f++)
                yield return a[f].Index;
            for (; s < b.Count; s++)
                yield return b[s].Index;
        }

        private static void Dfs(Node cur, HashSet<Node> visited, Func<Node, IList<Node>> edges, int fromIndex, int toIndex) 
        {
            if (cur.Index < fromIndex || cur.index > toIndex || visited.Contains(cur))
                return;
            visited.Add(cur);
            foreach (var next in edges(cur))
                Dfs(next, visited, edges);
        }

        public static IList<Node> InitGraph(int nVertices) 
        {
            return Enumerable.Range(0, nVertices).Select(i => new Node {Index = i}).ToList();
        }

        public static void AddEdge(IList<Node> topoSort, Node src, Node dest) 
        {
            if (src.Index < dest.Index) 
            {
                src.Outgoing.Add(dest);
                dest.Incoming.Add(src);
                return;
            }
            var deltaPlus = new HashSet<Node>();
            Dfs(dest, deltaPlus, n => n.Outgoing, dest.Index, src.Index);
            var deltaMinus = new HashSet<Node>();
            Dfs(src, deltaMinus, n => n.Incoming, dest.Index, src.Index);
            var deltaPlusList = deltaPlus.OrderBy(node => node.Index).ToList();
            deltaPlus.Intersect(deltaMinus);
            if (deltaPlus.Count > 0)
                throw new InvalidOperationException("The edge to be added would introduce a cycle");
            src.Outgoing.Add(dest);
            dest.Incoming.Add(src);
            var deltaMinusList = deltaMinus.OrderBy(node => node.Index).ToList();
            var cEntry = 0;
            foreach (var index in Merge(deltaMinusList, deltaPlusList)) 
            {
                var cNode = cEntry < deltaMinusList.Count ? deltaMinusList[cEntry] : deltaPlusList[cEntry - deltaMinusList.Count];
                cEntry++;
                topoSort[index] = cNode;
                cNode.Index = index;
            }
        } 
    }
}