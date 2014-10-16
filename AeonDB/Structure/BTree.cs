using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Structure
{
    public class BTree : IDisposable
    {
        private const int HeaderSize = sizeof(uint) + sizeof(long) + sizeof(long); // order + nodeSize + rootPosition

        private BTreeNode root;
        private uint order;
        private long nodeSize;
        private string fileName;
        private FileStream file;

        /// <summary>
        /// Creates a new BTree at the specified location, with the specified order.
        /// </summary>
        /// <param name="fileName">The file path for the BTree.</param>
        /// <param name="order">The order of the BTree.</param>
        public BTree(string fileName, uint order)
        {
            this.fileName = fileName;
            this.order = order;
            this.root = new BTreeNode(this);
            this.nodeSize = sizeof(long) * ((2 * this.order) - 1); // Keys
            this.nodeSize += sizeof(ulong) * ((2 * this.order) - 1); // Values
            this.nodeSize += sizeof(uint) + sizeof(bool); // valueCount and isLeaf
            this.nodeSize += sizeof(ulong) * 2 * order; // children positions

            this.file = null;
            this.Initialise();
        }

        /// <summary>
        /// Initialises a new BTree from the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        public BTree(string fileName)
        {
            this.fileName = fileName;
            this.Load();
        }

        internal uint Order
        {
            get { return this.order; }
        }

        internal long NodeSize
        {
            get { return this.nodeSize; }
        }

        private void Save(bool headerOnly)
        {
            this.file.Seek(HeaderSize, SeekOrigin.Begin);

            if (!headerOnly)
            {
                this.root.Save(this.file);
            }

            this.file.Seek(0, SeekOrigin.Begin);
            var header = new byte[HeaderSize];
            using (var ms = new MemoryStream(header))
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(this.order);
                    bw.Write(this.nodeSize);
                    bw.Write(this.root.Position);
                }
            }

            file.Write(header, 0, header.Length);
            file.Flush();
        }

        public void Open()
        {
            this.file = new FileStream(this.fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }

        public void Close()
        {
            if (this.file == null)
            {
                return;
            }

            this.file.Close();
            this.file = null;
        }

        private void Load()
        {
            this.file = new FileStream(this.fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            var header = new byte[HeaderSize];
            file.Read(header, 0, HeaderSize);

            using (var ms = new MemoryStream(header))
            {
                using (var br = new BinaryReader(ms))
                {
                    this.order = br.ReadUInt32();
                    this.nodeSize = br.ReadInt64();
                    var rootPosition = br.ReadInt64();
                    this.root = new BTreeNode(this, rootPosition);
                }
            }
            
            this.root.Load(file);

            this.file.Close();
            this.file = null;
        }

        private void Initialise()
        {
            if (!File.Exists(this.fileName))
            {
                this.file = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                this.Save(false);
                this.Close();
            }

            this.Load();
        }

        public void Insert(long key, long value)
        {
            if (this.root.ValueCount == ((2 * this.Order) - 1))
            {
                // Node is full, need to split
                var oldRoot = this.root;
                var newNode = new BTreeNode(this);
                this.root = newNode;
                this.root.SetNewRoot(oldRoot, this.file);

                // Update the header for the new root.
                this.Save(true);

                this.root.Split(oldRoot, 0, file);
                this.root.Insert(key, value, file);

                // Only need to leave root in memory so can dispose the old root.
                this.root.DisposeChildren();
            }
            else
            {
                this.root.Insert(key, value, file);
            }
        }

        public long this[long key]
        {
            get
            {
                return this.root.GetValue(key, this.file);
            }
        }

        public void Dispose()
        {
            this.Close();
            this.root.Dispose();
            this.root = null;
        }
    }
}
