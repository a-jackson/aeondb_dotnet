using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AeonDB.Structure;
using System.Diagnostics;

namespace AeonDB.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            int max = 1000000;
            var tree = new BTree("test",100);

            tree.Open();
            //for (long i = 1; i < max; i++)
            {
                //tree.Insert(i, i * i);
            }

            //for (long i = 1; i < max; i++)
            {
                //Console.WriteLine(tree[i]);
            }

            List<long> ticks = new List<long>();
            var rnd = new Random();

            for (int i = 0; i < 10000; i++)
            {

                var num = rnd.Next(max);
                Stopwatch sw = Stopwatch.StartNew();
                var value = tree[num];

                sw.Stop();

                Console.WriteLine("{0},{1},{2}", num, value, sw.ElapsedTicks);

                ticks.Add(sw.ElapsedTicks);
            }

            tree.Close();

            Console.WriteLine(ticks.Average());
            //System.IO.File.Delete("test");
        }
    }
}
