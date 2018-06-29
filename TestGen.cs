using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicTopoSort
{
    static class TestGen
    {
        public static IEnumerable<(int, int)> GenerateWorstCase(int n) 
        {
            n -= n % 2;
            for (int i = 0; i < n / 2 - 1; i++)
                for (int j = i+1; j < n / 2; j++)
                    yield return (i, j);
            for (int i = n/2; i < n - 1; i++)
                for (int j = i+1; j < n; j++)
                    yield return (j, i);
            yield return (n - 1, 0);
        }

        public static IEnumerable<(int, int)> GenerateRandomTest(int n)
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

            for (int i = rng.Next((int) Math.Min((long) n * (n - 1) / 2, 100000)); i >= 0; i--)
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
