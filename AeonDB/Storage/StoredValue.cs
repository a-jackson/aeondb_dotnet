using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    public abstract class StoredValue
    {
        public Timestamp Time { get; internal set; }
        public object Value { get; internal set; }

        internal abstract void ReadValueFromPage(BinaryReader br);
        internal abstract void SaveValueToPage(BinaryWriter bw);
    }
}
