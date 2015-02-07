using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Utility
{
    /// <summary>
    /// Type to represent a position in a file.
    /// </summary>
    internal class Position : SemanticType<long>
    {
        public const int TypeSize = sizeof(long);

        public Position(long position) : base(pos => pos > 0, position)
        {
        }
    }
}
