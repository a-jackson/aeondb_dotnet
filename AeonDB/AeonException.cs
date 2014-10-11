using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB
{
    public class AeonException : Exception
    {
        internal AeonException(string message) : base(message) { }
        internal AeonException(string message, Exception innerException) : base(message, innerException) { }
    }
}
