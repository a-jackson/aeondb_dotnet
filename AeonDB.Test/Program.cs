using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AeonDB.Structure;

namespace AeonDB.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new BTree("test", 10);

            tree.Open();
            for (long i = 1; i < 50000; i++)
            {
                tree.Insert(i, i * i);
            }

            
            for (long i = 1; i < 50000; i++)
            {
                Console.WriteLine(tree[i]);
            }

            tree.Close();
        }
    }
}
