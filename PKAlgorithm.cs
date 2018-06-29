using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicTopoSort
{
    public class PkAlgorithm : TopoSortAlgoBase
    {
        private static void Dfs(Node cur, HashSet<Node> visited, Func<Node, IList<Node>> edges, int fromIndex, int toIndex) 
        {
            if (cur.Index < fromIndex || cur.Index > toIndex || visited.Contains(cur))
                return;
            visited.Add(cur);
            foreach (var next in edges(cur))
                Dfs(next, visited, edges, fromIndex, toIndex);
        }

        private readonly HashSet<Node> _deltaPlus = new HashSet<Node>();
        private readonly HashSet<Node> _deltaMinus = new HashSet<Node>();
        private readonly List<Node> _deltaPlusList = new List<Node>();
        private readonly List<Node> _deltaMinusList = new List<Node>();
        private readonly List<int> _merged = new List<int>();

        public override void AddEdge(Node src, Node dest)
        {
            for (; SortedNodes.Count < Nodes.Count;)
                SortedNodes.Add(Nodes[SortedNodes.Count]);
            if (src.Index < dest.Index) 
            {
                src.Outgoing.Add(dest);
                dest.Incoming.Add(src);
                return;
            }
            _deltaPlus.Clear();
            Dfs(dest, _deltaPlus, n => n.Outgoing, dest.Index, src.Index);
            _deltaMinus.Clear();
            Dfs(src, _deltaMinus, n => n.Incoming, dest.Index, src.Index);
            _deltaPlusList.Clear();
            _deltaPlusList.AddRange(_deltaPlus.OrderBy(node => node.Index));
            _deltaPlus.IntersectWith(_deltaMinus);
            if (_deltaPlus.Count > 0)
                throw new InvalidOperationException("The edge to be added would introduce a cycle");
            src.Outgoing.Add(dest);
            dest.Incoming.Add(src);
            _deltaMinusList.Clear();
            _deltaMinusList.AddRange(_deltaMinus.OrderBy(node => node.Index));
            _merged.Clear();
            int f = 0, s = 0;
            while (f < _deltaPlusList.Count && s < _deltaMinusList.Count) 
            {
                if (_deltaPlusList[f].Index < _deltaMinusList[s].Index) 
                {
                    _merged.Add(_deltaPlusList[f].Index);
                    f++;
                } 
                else 
                {
                    _merged.Add(_deltaMinusList[s].Index);
                    s++;
                }
            }
            for (; f < _deltaPlusList.Count; f++)
                _merged.Add(_deltaPlusList[f].Index);
            for (; s < _deltaMinusList.Count; s++)
                _merged.Add(_deltaMinusList[s].Index);
            var cEntry = 0;
            for (int q = 0; q < _merged.Count; q++)
            {
                var cNode = cEntry < _deltaMinusList.Count ? _deltaMinusList[cEntry] : _deltaPlusList[cEntry - _deltaMinusList.Count];
                cEntry++;
                SortedNodes[_merged[q]] = cNode;
            }

            for (int q = 0; q < _merged.Count; q++)
                SortedNodes[_merged[q]].Index = _merged[q];
        } 
    }
}