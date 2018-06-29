using System;
using System.Collections.Generic;

namespace DynamicTopoSort
{
    class NaiveImplementation: TopoSortAlgoBase
    {
        private readonly Dictionary<Node, bool> _states = new Dictionary<Node, bool>();

        private void TopoSort(Node cur)
        {
            if (_states.TryGetValue(cur, out var b))
            {
                if (!b)
                    throw new InvalidOperationException("A cycle has been encountered");
                return;
            }

            _states[cur] = false;
            for (var i = 0; i < cur.Outgoing.Count; i++)
                TopoSort(cur.Outgoing[i]);
            _states[cur] = true;
            SortedNodes.Add(cur);
        }

        public override void AddEdge(Node src, Node dest)
        {
            src.Outgoing.Add(dest);
            dest.Incoming.Add(src);
            _states.Clear();
            SortedNodes.Clear();
            try
            {
                for (var i = 0; i < Nodes.Count; i++)
                    TopoSort(Nodes[i]);
            }
            catch
            {
                src.Outgoing.RemoveAt(src.Outgoing.Count - 1);
                dest.Incoming.RemoveAt(dest.Incoming.Count - 1);
                throw;
            }

            SortedNodes.Reverse();
            for (int i = 0; i < SortedNodes.Count; i++)
                SortedNodes[i].Index = i;
        }
    }
}
