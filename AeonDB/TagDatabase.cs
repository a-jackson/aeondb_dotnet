using AeonDB.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AeonDB
{
    internal class TagDatabase
    {
        private const string TagDatabaseFileName = "TagDb.xml";

        private AeonDB aeonDb;
        private Dictionary<string, Tag> tags;

        internal TagDatabase(AeonDB aeonDb)
        {
            this.aeonDb = aeonDb;
            this.Initialise();
        }

        internal Tag CreateNewTag(string name, TagType type)
        {
            if (this.tags.ContainsKey(name)) {
                throw new AeonException("Tag already exists");
            }

            var tag = CreateTag(name, type);
            tag.TagId = this.tags.Values.Max(x => x.TagId) + 1;
            this.tags.Add(name, tag);
            this.SaveTags();

            return tag;
        }

        internal Tag this[string name]
        {
            get
            {
                return this.tags[name];
            }
        }

        private Tag CreateTag(string name, TagType type)
        {
            Tag newTag;

            switch (type)
            {
                case TagType.Double:
                    newTag = new DoubleTag(name);
                    break;
                case TagType.Float:
                    newTag = new FloatTag(name);
                    break;
                case TagType.Boolean:
                    newTag = new BooleanTag(name);
                    break;
                case TagType.Int16:
                    newTag = new Int16Tag(name);
                    break;
                case TagType.Int32:
                    newTag = new Int32Tag(name);
                    break;
                case TagType.Int64:
                    newTag = new Int64Tag(name);
                    break;
                default:
                    throw new AeonException("Unrecognised tag type.");
            }

            return newTag;
        }

        private void Initialise()
        {
            var dbFile = this.aeonDb.Directory + TagDatabaseFileName;
            XDocument db;

            if (!File.Exists(dbFile))
            {
                // This is a new database. We need to initialise the file.
                db = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("TagDb"));
                db.Save(dbFile);
            }
            else
            {
                db = XDocument.Load(dbFile);
            }

            this.tags = db.Root.Elements("Tag").Select(x =>
                {
                    var name = x.Attribute("name").Value;
                    var type = (TagType)Enum.Parse(typeof(TagType), x.Attribute("type").Value);
                    var tag = CreateTag(name, type);
                    tag.TagId = int.Parse(x.Attribute("id").Value);
                    return tag;
                }).ToDictionary(x => x.Name);
        }

        private void SaveTags()
        {
            var dbFile = this.aeonDb.Directory + TagDatabaseFileName;
            var db = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("TagDb", 
                        this.tags.Select(x => 
                            new XElement("Tag", 
                                new XAttribute("name", x.Value.Name), 
                                new XAttribute("type", x.Value.Type.ToString()),
                                new XAttribute("id", x.Value.TagId)))));
            db.Save(dbFile);
        }
    }
}
