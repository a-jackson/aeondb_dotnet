using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    public class Int16Value : StoredValue
    {
        public Int16Value()
        {
            this.Time = new Timestamp(0);
            this.Value = default(short);
        }

        internal override void ReadValueFromPage(BinaryReader br)
        {
            this.Time = new Timestamp(br.ReadInt64());
            this.Value = br.ReadInt16();
        }

        internal override void SaveValueToPage(BinaryWriter bw)
        {
            bw.Write(this.Time);
            bw.Write((short)this.Value);
        }
    }
}
