using ComunicadorSefaz.Models;
using ComunicadorSefaz.Services.Interfaces;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace ComunicadorSefaz.Services
{
    public class ComunicadorSefazService : IComunicadorSefazService
    {
        private readonly IMontarXmlEnvioService _montarXmlEnvioService;

        public ComunicadorSefazService(
            IMontarXmlEnvioService montarXmlEnvioService)
        {
            _montarXmlEnvioService = montarXmlEnvioService;
        }

        public async Task<object> EnviarManifestoAsync(X509Certificate2 certificado, string chaveNota)
        {

            var eventoDestinatarioModel = new EventoDestinatarioModel
            {
                IdLote = 1,
                tpAmb = 2,
                versao = "4.00",
                cUF = 52,
                versaoSchema = "PL_009",
                CNPJ = "",
                CPF = "",
                ChaveDeAcesso = chaveNota,
                TipoDoProcesso = "210200"
            };

            var url = $"https://hom1.nfe.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx";

            var endPointAddress = new EndpointAddress(url);

            //Necessario o certificado para acessar o serviço, se ja obteve o certificado, importe o serviço e
            //descomente as linhas abaixo

            //var webServiceClient = new NFeRecepcaoEvento4.NFeRecepcaoEvento4SoapClient(Binding, endPointAddress)
            //{
            //    ClientCredentials =
            //    {
            //        ClientCertificate =
            //        {
            //            Certificate = certificado
            //        }
            //    }
            //};

            var xml = _montarXmlEnvioService.ObterXmlDoManifesto(eventoDestinatarioModel, certificado);

            //var envio = new NFeRecepcaoEvento4.nfeRecepcaoEventoNFRequest()
            //{
            //    nfeDadosMsg = xml
            //};

            //var retorno = await webServiceClient.nfeRecepcaoEventoNFAsync(envio);
            var retornoManifesto = new RetornoEventosModel();

            //retornoManifesto = _montarXmlEnvioService.ConverterRetornoDoManifesto(envio.nfeDadosMsg.InnerXml, retorno.nfeRecepcaoEventoNFResult.OuterXml);

            return retornoManifesto;


        }

        private static CustomBinding Binding
        {
            get
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var textEncoding = new TextMessageEncodingBindingElement
                {
                    MessageVersion = MessageVersion.Soap11,
                    WriteEncoding = Encoding.UTF8,
                    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas
                    {
                        MaxDepth = 32,
                        MaxStringContentLength = 8192,
                        MaxArrayLength = 16384,
                        MaxBytesPerRead = 4096,
                        MaxNameTableCharCount = 16384
                    }
                };

                var httpsTransport = new HttpsTransportBindingElement
                {
                    MaxReceivedMessageSize = 50 * 1024 * 1024,
                    AllowCookies = false,
                    AuthenticationScheme = System.Net.AuthenticationSchemes.Digest,
                    BypassProxyOnLocal = false,
                    KeepAliveEnabled = true,
                    TransferMode = TransferMode.Buffered,
                    RequireClientCertificate = true
                };

                return new CustomBinding(textEncoding, httpsTransport);
            }
        }
    }
}