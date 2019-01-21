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

            Console.WriteLine("{0} milliseconds", sw.ElapsedTicks / 10000.0);
        }
    }
}