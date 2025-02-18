using System.Text.RegularExpressions;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;

namespace ComunicadorSefaz.Utils
{
    public static class SerializarXmlUtils
    {

        public static string ClassToXmlString<T>(this T objeto)
        {
            XElement xml;
            var ser = new XmlSerializer(typeof(T));

            using (var memory = new MemoryStream())
            {
                using (TextReader tr = new StreamReader(memory, Encoding.UTF8))
                {
                    ser.Serialize(memory, objeto);
                    memory.Position = 0;
                    xml = XElement.Load(tr);
                    xml.Attributes()
                        .Where(x => x.Name.LocalName.Equals("xsd") || x.Name.LocalName.Equals("xsi"))
                        .Remove();
                }
            }

            var result = XElement.Parse(xml.ToString()).ToString(SaveOptions.DisableFormatting);

            return result;
        }

        public static T XmlStringToClass<T>(this string input) where T : class
        {
            var ser = new XmlSerializer(typeof(T));

            using (var sr = new StringReader(input))
                return (T)ser.Deserialize(sr);
        }


        public static T FileXmlToClasse<T>(this string arquivo) where T : class
        {
            if (!File.Exists(arquivo))
            {
                throw new FileNotFoundException("File " + arquivo + " not found!");
            }

            var serializador = new XmlSerializer(typeof(T));
            var stream = new FileStream(arquivo, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            try
            {
                return (T)serializador.Deserialize(stream);
            }
            finally
            {
                stream.Close();
            }
        }


        public static void ClasstoFileXml<T>(this T objeto, string arquivo)
        {
            var dir = Path.GetDirectoryName(arquivo);
            if (dir != null && !Directory.Exists(dir))
            {
                throw new DirectoryNotFoundException("Directory " + dir + " not found!");
            }

            var xml = ClassToXmlString(objeto);

            try
            {
                var stw = new StreamWriter(arquivo);
                stw.WriteLine(xml);
                stw.Close();
            }
            catch (Exception)
            {
                throw new Exception("Error, can't create file " + arquivo + "!");
            }
        }


        public static string GetNodeFromFileXml(string nomeDoNode, string arquivoXml)
        {
            var xmlDoc = XDocument.Load(arquivoXml);

            var xmlString = (from d in xmlDoc.Descendants()
                             where d.Name.LocalName == nomeDoNode
                             select d).FirstOrDefault();

            if (xmlString == null)
                throw new Exception($"object not {nomeDoNode} found in this file {arquivoXml}!");

            return xmlString.ToString();
        }

        public static string GetNodeFromStringXml(string nomeDoNode, string stringXml)
        {
            var s = stringXml;
            var xmlDoc = XDocument.Parse(s);
            var xmlString = (from d in xmlDoc.Descendants()
                             where d.Name.LocalName == nomeDoNode
                             select d).FirstOrDefault();

            if (xmlString == null)
                throw new Exception($"object not {nomeDoNode} founc in xml!");

            return xmlString.ToString();
        }

        public static XmlElement SerializeToXmlElement(this object o)
        {
            var doc = new XmlDocument();

            using (var writer = doc.CreateNavigator().AppendChild())
            {
                new XmlSerializer(o.GetType()).Serialize(writer, o);
            }

            if (doc.DocumentElement != null && doc.DocumentElement.NamespaceURI != string.Empty)
            {
                doc.LoadXml(doc.OuterXml.Replace(doc.DocumentElement.NamespaceURI, ""));

                doc.DocumentElement?.RemoveAllAttributes();

                doc.LoadXml(doc.OuterXml);
            }

            return doc.DocumentElement;
        }

  
        public static string RemoveAllNamespaces(this string xmlDocument)
        {
            var xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            const string STR_XML_PATTERN = @"xmlns(:\w+)?=""([^""]+)""|xsi(:\w+)?=""([^""]+)""";
            var xml = Regex.Replace(xmlDocumentWithoutNs.ToString(), STR_XML_PATTERN, "");

            return xml;
        }
      
        private static XElement RemoveAllNamespaces(this XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                var xElement = new XElement(xmlDocument.Name.LocalName)
                {
                    Value = xmlDocument.Value
                };

                foreach (var attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }

            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(RemoveAllNamespaces));
        }
    }
}
