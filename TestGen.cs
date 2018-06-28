using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicTopoSort
{
    class TestGen
    {
        public static IEnumerable<(int, int)> GenerateTest(int n)
        {
            var permutation = Enumerable.Range(0, n).ToArray();
            var rng = new Random();
            for (int i = 0; i < n - 1; i++)
            {
                var idx = rng.Next(i, n);
                var t = permutation[i];
                permutation[i] = permutation[idx];
                permutation[idx] = t;
            }

            for (int i = rng.Next((int) Math.Min((long) n * (n - 1) / 2, 1000000)); i >= 0; i--)
            {
                var x = rng.Next(n);
                var y = rng.Next(n - 1);
                if (y == x)
                    y++;
                yield return i > 0 ? (permutation[Math.Min(x, y)], permutation[Math.Max(x, y)]) : (permutation[Math.Max(x, y)], permutation[Math.Min(x, y)]);
            }
        }
    }
}
