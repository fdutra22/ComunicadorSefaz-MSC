using ComunicadorSefaz.Services.Interfaces;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace ComunicadorSefaz.Services
{
    public class AssinarXmlService : IAssinarXmlService
    {
        public string Assinar(X509Certificate2 certificado, string xmlFile, string tagAssinatura, string tagAtributoId)
        {
            try
            {
                var doc = new XmlDocument { PreserveWhitespace = false };
                doc.LoadXml(xmlFile);

                if (doc.GetElementsByTagName(tagAssinatura).Count == 0)
                {
                    throw new Exception("Signature Tag " + tagAssinatura.Trim() + " not exists in XML. (Error Code: 5)");
                }
                else if (doc.GetElementsByTagName(tagAtributoId).Count == 0)
                {
                    throw new Exception("Signature Tag " + tagAtributoId.Trim() + " not exists in XML. (Error Code: 4)");
                }
                // Existe mais de uma tag a ser assinada
                else
                {
                    var lists = doc.GetElementsByTagName(tagAssinatura);

                    foreach (XmlNode nodes in lists)
                    {
                        foreach (XmlNode childNodes in nodes.ChildNodes)
                        {
                            if (!childNodes.Name.Equals(tagAtributoId))
                                continue;

                            if (childNodes.NextSibling != null && childNodes.NextSibling.Name.Equals("Signature"))
                                continue;

                            // Create a reference to be signed
                            var reference = new Reference
                            {
                                Uri = string.Empty,
                                DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1"
                            };

                            // pega o uri que deve ser assinada                                       
                            var childElemen = (XmlElement)childNodes;
                            if (childElemen.GetAttributeNode("Id") != null)
                            {
                                var attributeNode = childElemen.GetAttributeNode("Id");

                                if (attributeNode != null)
                                    reference.Uri = "#" + attributeNode.Value;
                            }
                            else if (childElemen.GetAttributeNode("id") != null)
                            {
                                var attributeNode = childElemen.GetAttributeNode("id");

                                if (attributeNode != null)
                                    reference.Uri = "#" + attributeNode.Value;
                            }

                            var documentoNovo = new XmlDocument();
                            documentoNovo.LoadXml(nodes.OuterXml);

                            var signedXml = new SignedXml(documentoNovo)
                            {
                                SigningKey = certificado.PrivateKey
                            };

                            signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

                            var env = new XmlDsigEnvelopedSignatureTransform();
                            reference.AddTransform(env);

                            var c14 = new XmlDsigC14NTransform();
                            reference.AddTransform(c14);

                            signedXml.AddReference(reference);

                            var keyInfo = new KeyInfo();

                            keyInfo.AddClause(new KeyInfoX509Data(certificado));

                            signedXml.KeyInfo = keyInfo;
                            signedXml.ComputeSignature();

                            var xmlDigitalSignature = signedXml.GetXml();

                            nodes.AppendChild(doc.ImportNode(xmlDigitalSignature, true));
                        }
                    }

                    return doc.OuterXml;
                }
            }
            catch (CryptographicException ex)
            {
                
                Debug.WriteLine("Error CryptographicException was throw");
              
            }
        }


        public void AssinarSHA256(XmlDocument xmlDoc, string idDoEvento, X509Certificate2 certificado)
        {
            
            // https://docs.microsoft.com/en-us/dotnet/framework/whats-new/#Crypto462
            //
            // SignedXml support for SHA-2 hashing The .NET Framework 4.6.2 adds support
            // to the SignedXml class for RSA-SHA256, RSA-SHA384, and RSA-SHA512 PKCS#1
            // signature methods, and SHA256, SHA384, and SHA512 reference digest algorithms.
            //
            // Any programs that have registered a custom SignatureDescription handler into CryptoConfig
            // to add support for these algorithms will continue to function as they did in the past, but
            // since there are now platform defaults, the CryptoConfig registration is no longer necessary.
            //
            //// First of all, we need to register a SignatureDescription class that defines the DigestAlgorithm as SHA256.
            //// You have to reference the System.Deployment assembly in your project.
            //CryptoConfig.AddAlgorithm(
            //   typeof(System.Deployment.Internal.CodeSigning.RSAPKCS1SHA256SignatureDescription),
            //   "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
            // RSAPKCS1SHA256SignatureDescription -> Disponível desde .NET Framework 4.5

            SignedXml signedXml = new SignedXml(xmlDoc);

            // Add the key to the SignedXml document. 
            signedXml.SigningKey = certificado.GetRSAPrivateKey();   // Disponível desde .NET Framework 4.6
                                                                     //signedXml.SigningKey = GetRSAPrivateKey(certificate);

            //
            // https://docs.microsoft.com/en-us/dotnet/framework/whats-new/#Crypto462
            //
            // SignedXml support for SHA-2 hashing The .NET Framework 4.6.2 adds support
            // to the SignedXml class for RSA-SHA256, RSA-SHA384, and RSA-SHA512 PKCS#1
            // signature methods, and SHA256, SHA384, and SHA512 reference digest algorithms.
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA256Url; //"http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"

            // Create a reference to be signed. Pass "" to specify that
            // all of the current XML document should be signed.
            Reference reference = new Reference("#"+ idDoEvento);

            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform());
            reference.DigestMethod = SignedXml.XmlDsigSHA256Url; //""http://www.w3.org/2001/04/xmlenc#sha256"

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            signedXml.KeyInfo = new KeyInfo();
            // Load the certificate into a KeyInfoX509Data object
            // and add it to the KeyInfo object.
            signedXml.KeyInfo.AddClause(new KeyInfoX509Data(certificado));

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

            if (xmlDoc.FirstChild is XmlDeclaration)
                xmlDoc.RemoveChild(xmlDoc.FirstChild);
        }
    }
}