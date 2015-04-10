using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AeonDB.Structure;
using System.Diagnostics;
using System.IO;

namespace AeonDB.Test
{
    class Program
    {
        static DateTime startTime = new DateTime(2015, 1, 1, 0, 0, 0);

        static void Main(string[] args)
        {
            var db = new AeonDB("data");

            CreateTestData(db);

            for (int i = 0; i < 10; i++)
                QueryPerformance(db);
        }

        static void CreateTestData(AeonDB db)
        {
            if (File.Exists("data\\test"))
            {
                return;
            }

            using (var tag = db.CreateTag("test", TagType.Double))
            {
                for (int i = 0; i < 86400; i++)
                {
                    tag.AddValue(new Utility.Timestamp(startTime + new TimeSpan(0, 0, i)), (double)i, true);
                }
            }
        }

        static void QueryPerformance(AeonDB db)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var query = new Query(db);
            query.SetTag("test");
            query.SetStartTime(startTime.AddSeconds(15361));
            query.SetDuration(1000);

            foreach (var value in query)
            {
                //var dbl = (double)value.Value;
                var time = value.Time;

                //Console.WriteLine(time.Value + "\t" + dbl);
            }

            sw.Stop();

            Console.WriteLine("{0} microseconds", sw.ElapsedTicks / 10.0);
        }

        static void TreePerformance()
        {
            int max = 1000000;
            var tree = new BTree("test", 100);

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

                Console.WriteLine("{0},{1},{2}", num, value, sw.ElapsedTicks / 10.0);

                ticks.Add(sw.ElapsedTicks);
            }

            tree.Close();

            Console.WriteLine(ticks.Average());
            //System.IO.File.Delete("test");

        }
    }
}