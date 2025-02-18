using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace ComunicadorSefaz.Services.Interfaces
{
    public interface IAssinarXmlService
    {
        string Assinar(X509Certificate2 x509Certificate2, string xmlFile, string tagAssinatura, string tagAtributoId);

        void AssinarSHA256(XmlDocument xmlDoEvento, string idDoEvento, X509Certificate2 certificado);
    }
}