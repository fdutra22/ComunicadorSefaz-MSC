using ComunicadorSefaz.Models;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace ComunicadorSefaz.Services.Interfaces
{
    public interface IMontarXmlEnvioService
    {

        XmlNode ObterXmlDoManifesto(EventoDestinatarioModel eventoDestinatarioModel, X509Certificate2 certificado);
        
        EventoRetornoModel ConverterRetornoDoManifesto(string xmlDeEnvio, string xmlDoRetorno);    
     
    }
}
