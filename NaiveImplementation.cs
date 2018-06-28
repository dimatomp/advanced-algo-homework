using System;
using System.Collections.Generic;
using System.Linq;
using Node = DynamicTopoSort.PkAlgorithm.Node;

namespace DynamicTopoSort
{
    static class NaiveImplementation
    {
        private static void TopoSort(Node cur, Dictionary<Node, bool> states, IList<Node> result)
        {
            if (states.TryGetValue(cur, out var b))
            {
                if (!b)
                    throw new InvalidOperationException("A cycle has been encountered");
                return;
            }

            states[cur] = false;
            for (var i = 0; i < cur.Outgoing.Count; i++)
                TopoSort(cur.Outgoing[i], states, result);
            states[cur] = true;
            result.Append(cur);
        }

        public static List<Node> AddEdge(List<Node> nodes, Node src, Node dest)
        {
            var nAnswer = new List<Node>();
            var states = new Dictionary<Node, bool>();
            src.Outgoing.Add(dest);
            dest.Incoming.Add(src);
            try
            {
                for (var i = 0; i < nodes.Count; i++)
                    TopoSort(nodes[i], states, nAnswer);
            }
            catch
            {
                src.Outgoing.RemoveAt(src.Outgoing.Count - 1);
                dest.Incoming.RemoveAt(dest.Incoming.Count - 1);
                throw;
            }

            nAnswer.Reverse();
            for (int i = 0; i < nAnswer.Count; i++)
                nAnswer[i].Index = i;
            return nAnswer;
        }
    }
}
