using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    internal abstract class Page : IEnumerable<StoredValue>
    {
        /// <summary>
        /// The maximum number of values on a page.
        /// </summary>
        public const int PageValueCount = 60;

        /// <summary>
        /// The size of the pages's header.
        /// </summary>
        private static readonly Size HeaderSize = new Size(
            Timestamp.TypeSize  // pageTime
            + Timestamp.TypeSize // pageSize
            + sizeof(int)); // valueCount


        private Position position;
        private Timestamp pageTime;
        private int valueCount;
        private StoredValue[] values;

        protected Page(Position position)
        {
            this.position = position;
            this.values = new StoredValue[PageValueCount];
            for (int i = 0; i < PageValueCount; i++)
            {
                this.values[i] = GetNewValue();
            }
        }

        public Position Position
        {
            get { return position; }
        }

        public int ValueCount
        {
            get { return valueCount; }
        }

        public Timestamp PageTime
        {
            get { return this.pageTime; }
            internal set { this.pageTime = value; }
        }

        internal void Load(FileStream file)
        {
            var page = new byte[PageSize];
            file.Seek(this.position, SeekOrigin.Begin);
            file.Read(page, 0, PageSize);

            using (var ms = new MemoryStream(page))
            {
                using (var br = new BinaryReader(ms))
                {
                    this.pageTime = new Timestamp(br.ReadInt64());
                    var pageSize = new Size(br.ReadInt32());
                    if (pageSize != PageSize)
                    {
                        throw new AeonException("Possible corrupt timestore. Page Size not as expected.");
                    }

                    this.valueCount = br.ReadInt32();

                    for (int i = 0; i < PageValueCount; i++)
                    {
                        this.values[i] = GetNewValue();
                        this.values[i].ReadValueFromPage(br);
                    }
                }
            }
        }

        internal void AddValue(Timestamp timestamp, object value)
        {
            if (this.valueCount == 0)
            {
                // First value in page.
                this.pageTime = GetPageTimestamp(timestamp);
            }

            if (this.valueCount >= PageValueCount || timestamp >= this.PageTime + PageValueCount)
            {
                throw new InvalidOperationException("Page is full.");
            }

            this.values[this.valueCount].Time = timestamp;
            this.values[this.valueCount].Value = value;
            this.valueCount++;
        }

        internal Size PageSize
        {
            get { return new Size(HeaderSize + (PageValueCount * this.ValueSize) + (PageValueCount * Timestamp.TypeSize)); }
        }

        protected abstract Size ValueSize { get; }
        protected abstract StoredValue GetNewValue();

        internal void Save(FileStream file)
        {
            var page = new byte[PageSize];

            file.Seek(this.position, SeekOrigin.Begin);

            using (var ms = new MemoryStream(page))
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(this.pageTime);
                    bw.Write(this.PageSize);
                    bw.Write(this.valueCount);

                    for (int i = 0; i < PageValueCount; i++)
                    {
                        this.values[i].SaveValueToPage(bw);
                    }
                }
            }

            file.Write(page, 0, PageSize);
        }

        /// <summary>
        /// Returns the timestamp for the page that the specified timestamp would be on.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>The timestamp of the specified timestamp's page.</returns>
        internal static Timestamp GetPageTimestamp(Timestamp timestamp)
        {
            return new Timestamp(timestamp - (timestamp % PageValueCount));
        }

        public IEnumerator<StoredValue> GetEnumerator()
        {
            for (int i = 0; i < this.valueCount;i++)
            {
                yield return this.values[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
