using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    internal class Int32Page : Page
    {
        internal Int32Page(Position position)
            : base(position)
        {
        }

        protected override Size ValueSize
        {
            get { return new Size(sizeof(int)); }
        }

        protected override StoredValue GetNewValue()
        {
            return new Int32Value();
        }
    }
}
