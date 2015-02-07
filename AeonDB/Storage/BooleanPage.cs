using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    internal class BooleanPage : Page
    {
        internal BooleanPage(Position position) : base(position)
        {
        }

        protected override Size ValueSize
        {
            get { return new Size(sizeof(bool)); }
        }

        protected override StoredValue GetNewValue()
        {
            return new BooleanValue();
        }
    }
}
