using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    public class Int64Value : StoredValue
    {
        public Int64Value()
        {
            this.Time = new Timestamp(0);
            this.Value = default(long);
        }

        internal override void ReadValueFromPage(BinaryReader br)
        {
            this.Time = new Timestamp(br.ReadInt64());
            this.Value = br.ReadInt64();
        }

        internal override void SaveValueToPage(BinaryWriter bw)
        {
            bw.Write(this.Time);
            bw.Write((long)this.Value);
        }
    }
}
