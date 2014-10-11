using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Structure
{
    internal class BTreeNode : IDisposable
    {
        private BTree tree;
        private long[] keys;
        private long[] values;
        private uint valueCount;
        private bool isLeaf;
        private BTreeNode[] children;
        private long position;
        private long[] childrenPositions;

        internal BTreeNode(BTree tree, long position)
        {
            this.tree = tree;
            this.keys = new long[(2 * tree.Order) - 1];
            this.values = new long[(2 * tree.Order) - 1];
            this.valueCount = 0;
            this.isLeaf = true;
            this.children = new BTreeNode[2 * tree.Order];
            this.childrenPositions = new long[2 * tree.Order];
            this.position = position;
        }

        internal BTreeNode(BTree tree)
            : this(tree, 0)
        {
        }

        public long Position
        {
            get { return this.position; }
        }

        public uint ValueCount
        {
            get { return valueCount; }
        }

        internal void SetNewRoot(BTreeNode oldRoot, FileStream file)
        {
            this.isLeaf = false;
            this.children[0] = oldRoot;
            this.childrenPositions[0] = oldRoot.position;
            this.SaveNewNode(file);
        }

        internal void Save(FileStream file)
        {
            // If there's no position it must be new so add at the current location
            if (this.position != 0)
            {
                file.Seek(this.position, SeekOrigin.Begin);
            }
            else
            {
                this.position = file.Position;
            }

            var nodeBytes = new byte[tree.NodeSize];

            using (var ms = new MemoryStream(nodeBytes))
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(valueCount);
                    bw.Write(isLeaf);
                    for (int i = 0; i < ((2 * tree.Order) - 1); i++)
                    {
                        bw.Write(keys[i]);
                    }

                    for (int i = 0; i < ((2 * tree.Order) - 1); i++)
                    {
                        bw.Write(values[i]);
                    }

                    for (int i = 0; i < ((2 * tree.Order) - 1); i++)
                    {
                        bw.Write(this.childrenPositions[i]);
                    }
                }
            }

            file.Write(nodeBytes, 0, nodeBytes.Length);
            file.Flush();
        }

        internal void Load(FileStream file)
        {
            file.Seek(this.position, SeekOrigin.Begin);

            var br = new BinaryReader(file);
            this.valueCount = br.ReadUInt32();
            this.isLeaf = br.ReadBoolean();
            for (int i = 0; i < ((2 * tree.Order) - 1); i++)
            {
                this.keys[i] = br.ReadInt64();
            }

            for (int i = 0; i < ((2 * tree.Order) - 1); i++)
            {
                this.values[i] = br.ReadInt64();
            }

            for (int i = 0; i < ((2 * tree.Order) - 1); i++)
            {
                this.childrenPositions[i] = br.ReadInt64();
            }

            // Don't dispose the BinaryReader, we want to keep the FileStream open.
        }

        internal void Split(BTreeNode child, int medianIndex, FileStream file)
        {
            var newNode = new BTreeNode(this.tree);
            newNode.isLeaf = child.isLeaf;
            newNode.valueCount = this.tree.Order - 1;
            uint i;

            // Copy the higher order keys to the new child
            for (i = 0; i < tree.Order - 1; i++)
            {
                newNode.keys[i] = child.keys[i + tree.Order];
                newNode.values[i] = child.values[i + tree.Order];
                if (!child.isLeaf)
                {
                    newNode.children[i] = child.children[i + tree.Order];
                    newNode.childrenPositions[i] = child.childrenPositions[i + tree.Order];
                }
            }

            // Copy the last child
            if (!child.isLeaf)
            {
                newNode.children[i] = child.children[i + tree.Order];
                newNode.childrenPositions[i] = child.childrenPositions[i + tree.Order];
            }

            child.valueCount = tree.Order - 1;
            child.Save(file);

            // Move all the child up 1 to make room for the new node
            for (i = this.valueCount + 1; i > medianIndex; i--)
            {
                this.children[i] = this.children[i - 1];
                this.childrenPositions[i] = this.childrenPositions[i - 1];
            }

            newNode.SaveNewNode(file);

            this.children[medianIndex + 1] = newNode;
            this.childrenPositions[medianIndex + 1] = newNode.position;

            for (i = this.valueCount; i > medianIndex; i--)
            {
                this.keys[i] = this.keys[i - 1];
            }

            this.keys[medianIndex] = child.keys[tree.Order - 1];
            this.values[medianIndex] = child.keys[tree.Order - 1];
            this.valueCount++;
            this.Save(file);

            newNode.Dispose();
            this.children[medianIndex + 1] = null;
        }

        internal void Insert(long key, long value, FileStream file)
        {
            int i = (int)this.valueCount - 1;

            if (this.isLeaf)
            {
                // Loop down the nodes moving them all up 1 place until the space for the new value is reached.
                while (i >= 0 && key < this.keys[i])
                {
                    this.keys[i + 1] = this.keys[i];
                    this.values[i + 1] = this.values[i];
                    i--;
                }

                // Insert the new value.
                this.keys[i + 1] = key;
                this.values[i + 1] = value;
                this.valueCount++;
                this.Save(file);
            }
            else
            {
                // Node is full so need to identify the correct child.
                while (i >= 0 && key < this.keys[i])
                {
                    i--;
                }
                i++;

                // Load the child at i
                this.children[i] = new BTreeNode(tree, this.childrenPositions[i]);
                this.children[i].Load(file);

                if (this.children[i].valueCount == (2 * tree.Order) - 1)
                {
                    this.Split(this.children[i], i, file);

                    if (key > this.keys[i])
                    {
                        // Need the next child. Free this one.
                        this.children[i].Dispose();
                        this.children[i] = null;
                        i++;

                        //Load next child.
                        this.children[i] = new BTreeNode(tree, this.childrenPositions[i]);
                        this.children[i].Load(file);
                    }
                }

                this.children[i].Insert(key, value, file);

                this.children[i].Dispose();
                this.children[i] = null;
            }
        }

        internal long GetValue(long key, FileStream file)
        {
            int i = 0;
            while (i < this.valueCount && key > this.keys[i])
            {
                i++;
            }

            if (i < this.valueCount && key == this.keys[i])
            {
                return this.values[i];
            }
            else if (this.isLeaf)
            {
                throw new AeonException("Key not found.");
            }
            else
            {
                this.children[i] = new BTreeNode(this.tree, this.childrenPositions[i]);
                this.children[i].Load(file);
                var result = this.children[i].GetValue(key, file);
                this.children[i].Dispose();
                this.children[i] = null;
                return result;
            }
        }


        private void SaveNewNode(FileStream file)
        {
            file.Seek(0, SeekOrigin.End);
            this.Save(file);
        }

        public void Dispose()
        {
            this.keys = null;
            this.values = null;
            this.childrenPositions = null;
            this.DisposeChildren();

            this.children = null;
        }

        internal void DisposeChildren()
        {
            if (!this.isLeaf)
            {
                for (int i = 0; i < this.valueCount; i++)
                {
                    if (this.children[i] != null)
                    {
                        this.children[i].Dispose();
                        this.children[i] = null;
                    }
                }
            }
        }
    }
}
