﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicTopoSort
{
    class Program
    {
        private const int V = 40;

        static void GenTests(Func<int, IEnumerable<(int, int)>> testGen)
        {
            //var tasks = new List<Task>();
            for (int q = 1; q <= V; q++)
            {
                var i = q;
                //tasks.Add(Task.Run(() =>
                //{
                    var n = i * 10;
                    var edges = testGen(n).ToList();
                    using (var writer = new StreamWriter(new BufferedStream(new FileStream($"{i - 1:D2}.in", FileMode.Create))))
                    {
                        writer.WriteLine($"{n} {edges.Count}");
                        for (int j = 0; j < edges.Count; j++)
                            writer.WriteLine($"{edges[j].Item1} {edges[j].Item2}");
                    }
                //}));
            }

            //Task.WaitAll(tasks.ToArray());
        }

        static void TestAlgo(TopoSortAlgoBase algo)
        {
            for (int i = 1; i <= 40; i++)
            {
                using (var reader = new StreamReader(new BufferedStream(new FileStream($"{i - 1:D2}.in", FileMode.Open))))
                {
                    var nm = reader.ReadLine().Split().Select(int.Parse).ToArray();
                    algo.InitGraph(nm[0]);
                    var input = Enumerable.Range(0, nm[1])
                        .Select(j => reader.ReadLine().Split().Select(int.Parse).ToArray()).ToList();
                    var totalTime = new TimeSpan();
                    DateTime time;
                    for (int j = 0; j < nm[1] - 1; j++)
                    {
                        time = DateTime.Now;
                        algo.AddEdge(algo.Nodes[input[j][0]], algo.Nodes[input[j][1]]);
                        totalTime += DateTime.Now - time;
                        //foreach (var node in nodes)
                        //    foreach (var outgoing in node.Outgoing)
                        //        Debug.Assert(node.Index < outgoing.Index);
                    }

                    bool ok = false;
                    time = DateTime.Now;
                    try
                    {
                        algo.AddEdge(algo.Nodes[input[nm[1] - 1][0]], algo.Nodes[input[nm[1] - 1][1]]);
                    }
                    catch (InvalidOperationException)
                    {
                        ok = true;
                    }
                    finally
                    {
                        totalTime += DateTime.Now - time;
                    }
                    //Debug.Assert(ok);
                    Console.WriteLine($"{i - 1:D2}.in\t{nm[0]}\t{nm[1]}\t{totalTime.TotalSeconds}sec\t{totalTime.TotalMilliseconds / nm[1]}ms");
                }
            }
        }

        static void Main(string[] args)
        {
            TestAlgo(new PkAlgorithm());
        }
    }
}
