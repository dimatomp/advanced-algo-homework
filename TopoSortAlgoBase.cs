using System.Collections.Generic;

namespace DynamicTopoSort
{
    public abstract class TopoSortAlgoBase
    {
        public readonly List<Node> Nodes = new List<Node>();

        public void InitGraph(int nNodes)
        {
            Nodes.Clear();
            for (int i = 0; i < nNodes; i++)
                Nodes.Add(new Node{Index = i, Number = i});
        }

        public class Node
        {
            public readonly IList<Node> Outgoing = new List<Node>();
            public readonly IList<Node> Incoming = new List<Node>();
            public int Number;
            public int Index;

            public override string ToString()
            {
                return $"Number: {Number}, Index: {Index}";
            }
        }

        public readonly List<Node> SortedNodes = new List<Node>();
        public abstract void AddEdge(Node src, Node dest);
    }
}