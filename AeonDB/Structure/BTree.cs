using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Structure
{
    /// <summary>
    /// Class represents a BTree that is stored on the filesystem.
    /// Keys and values are both Int64.
    /// </summary>
    internal class BTree : IDisposable
    {
        /// <summary>
        /// The size of the file's header.
        /// </summary>
        private const int HeaderSize = sizeof(uint) // order
            + sizeof(long) // nodeSize
            + sizeof(long) // rootPosition
            + sizeof(int) // version
            + 8; // magic number

        private const long MagicNumber = 0x41454f4e494e4458;
        private const int Version = 1;

        private BTreeNode root;
        private uint order;
        private long nodeSize;
        private string fileName;
        private FileStream file;

        /// <summary>
        /// Creates a new BTree at the specified location, with the specified order.
        /// If the file already exists, <paramref name="order"/> will be ignored and 
        /// overwritten with the order of the existing tree on the file system.
        /// </summary>
        /// <param name="fileName">The file path for the BTree.</param>
        /// <param name="order">The order of the BTree.</param>
        internal BTree(string fileName, uint order)
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
        /// The file must already exist.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        internal BTree(string fileName)
        {
            this.fileName = fileName;
            this.Load();
        }

        /// <summary>
        /// Gets the order of the tree.
        /// </summary>
        internal uint Order
        {
            get { return this.order; }
        }

        /// <summary>
        /// Gets the number of bytes each node requires.
        /// </summary>
        internal long NodeSize
        {
            get { return this.nodeSize; }
        }

        /// <summary>
        /// Saves the tree to the file system.
        /// </summary>
        /// <param name="headerOnly">Indicates if only the header should be saved or the whole tree.</param>
        private void Save(bool headerOnly)
        {
            // Seek to after the header, the location of the first node.
            this.file.Seek(HeaderSize, SeekOrigin.Begin);

            // The root node must be saved first so we can confirm it's position in the file.
            // This is required for the header.
            if (!headerOnly)
            {
                this.root.Save(this.file);
            }

            // Seek the beginning of the file system for saving the header.
            this.file.Seek(0, SeekOrigin.Begin);

            // Create the header.
            // Uses a MemoryStream to Byte[] rather than writing directly to the filestream as 
            // the BinaryWriter closes the stream when it is disposed and we want to leave the 
            // open for future transactions.
            var header = new byte[HeaderSize];
            using (var ms = new MemoryStream(header))
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(MagicNumber);
                    bw.Write(Version);
                    bw.Write(this.order);
                    bw.Write(this.nodeSize);
                    bw.Write(this.root.Position);
                }
            }

            // Write to file.
            file.Write(header, 0, header.Length);
            file.Flush();
        }

        /// <summary>
        /// Opens the file. 
        /// Operation will fail if the BTree has not been initilised.
        /// </summary>
        internal void Open()
        {
            this.file = new FileStream(this.fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }

        /// <summary>
        /// Closes the tree's file if it is open.
        /// </summary>
        internal void Close()
        {
            if (this.file == null)
            {
                return;
            }

            this.file.Close();
            this.file = null;
        }

        /// <summary>
        /// Loads the tree's header and root node.
        /// </summary>
        private void Load()
        {
            this.file = new FileStream(this.fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            var header = new byte[HeaderSize];
            file.Read(header, 0, HeaderSize);

            long magicNumber;
            int version;

            using (var ms = new MemoryStream(header))
            {
                using (var br = new BinaryReader(ms))
                {
                    magicNumber = br.ReadInt64();
                    version = br.ReadInt32();
                    this.order = br.ReadUInt32();
                    this.nodeSize = br.ReadInt64();
                    var rootPosition = br.ReadInt64();
                    this.root = new BTreeNode(this, rootPosition);
                }
            }

            if (magicNumber != MagicNumber)
            {
                throw new AeonException("Unrecognised file format");
            }

            if (version < Version)
            {
                // Older format. Upgrade.
            }
            else if (version > Version)
            {
                throw new AeonException("Database created on newer of AeonDB. You must upgrade software to use.");
            }
            
            // Load the root node.
            // Only the root node is required to be held in memory. Other nodes will be loaded and 
            // disposed as required.
            this.root.Load(file);

            this.file.Close();
            this.file = null;
        }

        /// <summary>
        /// Intialises the tree on the file system if it does not already exist.
        /// </summary>
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

        /// <summary>
        /// Inserts the specified key and value into the tree.
        /// </summary>
        /// <param name="key">The key of the value to insert.</param>
        /// <param name="value">The value to insert.</param>
        internal void Insert(long key, long value)
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

        /// <summary>
        /// Retrieves the value for the specified key.
        /// </summary>
        /// <param name="key">The key to lookup.</param>
        /// <returns>The value of the specified tree.</returns>
        internal long this[long key]
        {
            get
            {
                return this.root.GetValue(key, this.file);
            }
        }

        /// <summary>
        /// Disposes of the tree and unloads all resources from memory.
        /// </summary>
        public void Dispose()
        {
            this.Close();
            this.root.Dispose();
            this.root = null;
        }
    }
}
