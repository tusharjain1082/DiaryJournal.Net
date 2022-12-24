using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace DiaryJournal.Net
{
    // this is the general indexing framework for the database indexing/id
    public class DatabaseIndexing
    {
        public Int64 currentDBIndex = 0;

        public const string dbIndexingFileName = "dbIndexing.bin";

        public static void fromFile(ref DatabaseIndexing dbIndexing, String file)
        {
            byte[] value = File.ReadAllBytes(file);
            dbIndexing.currentDBIndex = BitConverter.ToInt64(value, 0);
        }
        public static void toFile(ref DatabaseIndexing dbIndexing, String file)
        {
            byte[] value = BitConverter.GetBytes(dbIndexing.currentDBIndex);
            File.WriteAllBytes(file, value);
        }

    }

    public class DatabaseConfig
    {
        public String setName { get; set; } = "";
        public DateTime setDateTime { get; set; }
        public Guid setID { get; set; } = Guid.Empty;

//        public const string currentDBIndexTagName = "currentDBIndex";
        public const string configTagName = "config";
        public const string setNameTagName = "setName";
        public const string setDateTimeTagName = "setDateTime";
        public const string setIDTagName = "setID";

        public static void setDBConfig(ref DatabaseConfig config, String setName, DateTime setDateTime, Guid setID)
        {
            config.setName = setName;
            config.setDateTime = setDateTime;
            config.setID = setID;
        }

        // load config from input db set
        public static bool fromXml(ref DatabaseConfig config, String file)
        {
            if (!File.Exists(file))
                return false;

            // load stream
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            XmlElement root = doc.DocumentElement;

            XmlNodeList list = doc.GetElementsByTagName(configTagName);
            if (list.Count <= 0)
                return false;

            // primary element
            XmlElement configElement = (XmlElement)list[0];

            // get all child elements

            // set name
            list = configElement.GetElementsByTagName(setNameTagName);
            if (list.Count <= 0)
                return false;

            XmlElement child0 = (XmlElement)list[0];
            config.setName = child0.InnerText;

            // set date and time
            list = configElement.GetElementsByTagName(setDateTimeTagName);
            if (list.Count <= 0)
                return false;

            XmlElement child1 = (XmlElement)list[0];
            config.setDateTime = DateTime.ParseExact(child1.InnerText, "yyyy-MM-dd-HH-mm-ss-fff",
                  System.Globalization.CultureInfo.InvariantCulture);

            // set ID
            list = configElement.GetElementsByTagName(setIDTagName);
            if (list.Count <= 0)
                return false;

            XmlElement child2 = (XmlElement)list[0];
            config.setID = Guid.Parse(child2.InnerText);//UInt32.Parse(child2.InnerText);

            /*
            // current database index available for use
            list = configElement.GetElementsByTagName(currentDBIndexTagName);
            if (list.Count <= 0)
                return false;

            XmlElement child3 = (XmlElement)list[0];
            config.currentDBIndex = Int64.Parse(child3.InnerText);
            */

            return true;
        }

        public static String toXml(ref DatabaseConfig config)
        {
            //xml Decalration:
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            XmlElement root = doc.DocumentElement;

            // create entry
            XmlElement configElement = doc.CreateElement(string.Empty, configTagName, string.Empty);
            doc.AppendChild(configElement);

            XmlElement child0 = doc.CreateElement(string.Empty, setNameTagName, string.Empty);
            child0.InnerText = config.setName;
            configElement.AppendChild(child0);
            XmlElement child1 = doc.CreateElement(string.Empty, setDateTimeTagName, string.Empty);
            child1.InnerText = config.setDateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            configElement.AppendChild(child1);
            XmlElement child2 = doc.CreateElement(string.Empty, setIDTagName, string.Empty);
            child2.InnerText = config.setID.ToString();
            configElement.AppendChild(child2);
            /*
            XmlElement child3 = doc.CreateElement(string.Empty, currentDBIndexTagName, string.Empty);
            child3.InnerText = config.currentDBIndex.ToString();
            configElement.AppendChild(child3);
            */

            TextWriter writer = new StringWriterWithEncoding(Encoding.UTF8);
            doc.Save(writer);
            writer.Flush();
            String output = writer.ToString();
            writer.Close();
            writer.Dispose();
            return output;
        }

        public static String toXmlFile(ref DatabaseConfig config, String file)
        {
            String xml = toXml(ref config);
            File.WriteAllText(file, xml);
            return xml;
        }
        public sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding encoding;

            public StringWriterWithEncoding() : this(Encoding.UTF8) { }

            public StringWriterWithEncoding(Encoding encoding)
            {
                this.encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return encoding; }
            }
        }

    }
}
