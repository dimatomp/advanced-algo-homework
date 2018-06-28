using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicTopoSort
{
    static class BucketAlgorithm
    {
        public abstract class ArrayElement
        {
            public LinkedListNode<ArrayElement> PlaceInArray;
        }

        public class Node: ArrayElement
        {
            public readonly HashSet<Node> SAncestors = new HashSet<Node>();
            public readonly HashSet<Node> SDescendants = new HashSet<Node>();
            public readonly HashSet<Node> CycleAncestors = new HashSet<Node>();
            public readonly List<Node> Incoming = new List<Node>();
            public readonly List<Node> Outgoing = new List<Node>();
            public HashSet<Node> Ancestors;
            public HashSet<Node> Descendants;
            public readonly int Index;

            public Node(int index)
            {
                Index = index;
            }

            public bool IsS => Ancestors != null;

            public override int GetHashCode()
            {
                return Index;
            }
        }

        public class Bucket : ArrayElement
        {
        }

        public class Structure
        {
            public readonly List<Node> Nodes = new List<Node>();
            public readonly List<Node> SNodes = new List<Node>();
            public readonly Queue<Node> CycleToExplore = new Queue<Node>();
            public readonly HashSet<Node> Visited = new HashSet<Node>();
            public readonly LinkedList<ArrayElement> SortedList = new LinkedList<ArrayElement>();
            public readonly Dictionary<Node, (int, int)> OldBuckets = new Dictionary<Node, (int, int)>();
            public readonly Dictionary<(int, int), List<Node>> NewBucketsUp = new Dictionary<(int, int), List<Node>>();
            public readonly Dictionary<(int, int), List<Node>> NewBucketsDown = new Dictionary<(int, int), List<Node>>();
            public Bucket[,] Buckets;
        }

        public static Structure InitGraph(int nVertices)
        {
            var result = new Structure();
            var rng = new Random();
            var boundary = 11 * Math.Log(nVertices, 2) / Math.Sqrt(nVertices);
            for (int i = 0; i < nVertices; i++)
            {
                var node = new Node(i);
                result.Nodes.Add(node);
                if (rng.NextDouble() < boundary)
                {
                    node.Ancestors = new HashSet<Node> {node};
                    node.Descendants = new HashSet<Node> {node};
                    node.SAncestors.Add(node);
                    node.SDescendants.Add(node);
                    result.SNodes.Add(node);
                }
            }

            var upperIndex = (int) (12 * Math.Log(nVertices) * Math.Sqrt(nVertices)) + 1;
            result.Buckets = new Bucket[upperIndex,upperIndex];
            for (var a = 0; a < upperIndex; a++)
                for (var b = upperIndex - 1; b >= 0; b--)
                {
                    result.Buckets[a, b] = new Bucket();
                    result.Buckets[a, b].PlaceInArray = result.SortedList.AddLast(result.Buckets[a, b]);
                    if (a == 0 && b == 0)
                        for (var i = 0; i < nVertices; i++)
                            result.Nodes[i].PlaceInArray = result.SortedList.AddLast(result.Nodes[i]);
                }

            return result;
        }

        private static bool DfsHasPath(Node from, Node to, HashSet<Node> visited)
        {
            if (from == to)
                return true;
            if (visited.Contains(from))
                return false;
            visited.Add(from);
            for (int i = 0; i < from.Outgoing.Count; i++)
                if (DfsHasPath(from.Outgoing[i], to, visited))
                    return true;
            return false;
        }

        private static void DfsDist(Node cur, Node sNode, HashSet<Node> visited, Func<Node, List<Node>> edges, ICollection<Node> sSet, ICollection<Node> sSSet, Func<Node, ICollection<Node>> oppSet, Dictionary<Node, (int, int)> oldBuckets)
        {
            if (visited.Contains(cur) || sSet.Contains(cur))
                return;
            visited.Add(cur);
            if (!oldBuckets.ContainsKey(cur))
                oldBuckets[cur] = (cur.SAncestors.Count, cur.SDescendants.Count);
            sSet.Add(cur);
            if (cur.IsS)
                sSSet.Add(cur);
            oppSet(cur).Add(sNode);
            foreach (var next in edges(cur))
                DfsDist(next, sNode, visited, edges, sSet, sSSet, oppSet, oldBuckets);
        }

        public static void AddEdge(Structure result, Node from, Node to)
        {
            result.OldBuckets.Clear();
            result.NewBucketsUp.Clear();
            result.NewBucketsDown.Clear();
            for (int i = 0; i < result.SNodes.Count; i++)
            {
                var sNode = result.SNodes[i];
                if (sNode.Ancestors.Contains(to))
                {
                    DfsDist(from, sNode, result.Visited, n => n.Incoming, sNode.Ancestors, sNode.SAncestors, n => n.SDescendants, result.OldBuckets);
                    result.Visited.Clear();
                }

                if (sNode.Descendants.Contains(from))
                {
                    DfsDist(to, sNode, result.Visited, n => n.Outgoing, sNode.Descendants, sNode.SDescendants, n => n.Ancestors, result.OldBuckets);
                    result.Visited.Clear();
                }
            }
            result.CycleToExplore.Enqueue(to);
            while (result.CycleToExplore.Count > 0)
            {
                var cur = result.CycleToExplore.Dequeue();
                if (cur == from || from.CycleAncestors.Contains(cur))
                {
                    result.Visited.Clear();
                    if (DfsHasPath(from, to, result.Visited))
                    {
                        result.CycleToExplore.Clear();
                        throw new InvalidOperationException("A cycle was encountered");
                    }
                    break;
                }

                if (cur.CycleAncestors.Contains(@from) || cur.SAncestors.Count != @from.SAncestors.Count ||
                    cur.SDescendants.Count != @from.SDescendants.Count)
                    continue;

                cur.CycleAncestors.Add(from);
                for (int i = 0; i < cur.Outgoing.Count; i++)
                    result.CycleToExplore.Enqueue(cur.Outgoing[i]);
            }
            from.Outgoing.Add(to);
            to.Incoming.Add(from);

            // TODO Should be some kind of an efficient ordered list here
            foreach (var node in result.SortedList.OfType<Node>())
                if (result.OldBuckets.TryGetValue(node, out var oldKey))
                {
                    var key = (node.SAncestors.Count, node.SDescendants.Count);
                    var comparison = key.Item1 == oldKey.Item1 ? oldKey.Item2.CompareTo(key.Item2) : key.Item1.CompareTo(oldKey.Item1);
                    if (comparison == 0)
                        continue;
                    var newBuckets = comparison > 0 ? result.NewBucketsUp : result.NewBucketsDown;
                    if (!newBuckets.TryGetValue(key, out var v))
                        newBuckets[key] = v = new List<Node>();
                    v.Add(node);
                }

            foreach (var kv in result.NewBucketsUp)
                for (int i = kv.Value.Count - 1; i >= 0; i--)
                {
                    result.SortedList.Remove(kv.Value[i].PlaceInArray);
                    kv.Value[i].PlaceInArray = result.SortedList.AddAfter(result.Buckets[kv.Key.Item1, kv.Key.Item2].PlaceInArray, kv.Value[i]);
                }

            foreach (var kv in result.NewBucketsDown)
            {
                var next = kv.Key.Item2 > 0
                    ? (kv.Key.Item1, kv.Key.Item2 - 1)
                    : (kv.Key.Item1 + 1, result.Buckets.GetLength(1) - 1);
                for (var i = 0; i < kv.Value.Count; i++)
                {
                    result.SortedList.Remove(kv.Value[i].PlaceInArray);
                    kv.Value[i].PlaceInArray = next.Item1 < result.Buckets.GetLength(0)
                        ? result.SortedList.AddBefore(result.Buckets[next.Item1, next.Item2].PlaceInArray, kv.Value[i])
                        : result.SortedList.AddLast(kv.Value[i]);
                }
            }
        }
    }
}
