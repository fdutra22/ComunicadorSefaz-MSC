using ComunicadorSefaz.Models;
using ComunicadorSefaz.Services.Interfaces;
using ComunicadorSefaz.Utils;
using ComunicadorSefaz.Utils.Extensions;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace ComunicadorSefaz.Services
{
    public class MontarXmlEnvioService : IMontarXmlEnvioService
    {
        private readonly IAssinarXmlService _assinador;

        public MontarXmlEnvioService(IAssinarXmlService assinador)
        {
            _assinador = assinador;
        }

        public XmlNode ObterXmlDoManifesto(EventoDestinatarioModel requisicao, X509Certificate2 certificado)
        {
            var evento = new ManifestoDoDestinatario.TEnvEvento
            {
                idLote = requisicao.IdLote.ToString(),
                evento = ObterEventoDeManifesto(requisicao),
                versao = "1.00"
            };

            var cartaDecorrecaoComoString = evento.ClassToXmlString();

            var xmlAssinado = _assinador.Assinar(certificado, cartaDecorrecaoComoString, "evento", "infEvento");
            var xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(xmlAssinado);
            return xmlDocument;
        }


        private ManifestoDoDestinatario.TEvento[] ObterEventoDeManifesto(EventoDestinatarioModel eventoDeManifesto)
        {
            var manifesto = new ManifestoDoDestinatario.TEvento
            {
                infEvento = ObterInformacaoEventoManifesto(eventoDeManifesto),
                versao = "1.00"
            };

            return new[] { manifesto };
        }

        private ManifestoDoDestinatario.TEventoInfEvento ObterInformacaoEventoManifesto(EventoDestinatarioModel eventoDeManifesto)
        {
            const string NUMERO_DE_SEQUENCIA = "1";

            var infoManifesto = new ManifestoDoDestinatario.TEventoInfEvento
            {
                cOrgao = eventoDeManifesto.cUF.ParseEnumItem<ManifestoDoDestinatario.TCOrgaoIBGE>("cOrgao"),
                tpAmb = eventoDeManifesto.tpAmb.ParseEnumItem<ManifestoDoDestinatario.TAmb>("tpAmb"),

                Item = !string.IsNullOrEmpty(eventoDeManifesto.CPF)
               ? eventoDeManifesto.CPF?.ToString().Replace(".", "").Replace("-", "").Replace(" ", "").Trim()
                : eventoDeManifesto.CNPJ?.ToString().Replace(".", "").Replace("-", "").Replace(" ", "").Trim(),

                ItemElementName = !string.IsNullOrEmpty(eventoDeManifesto.CPF)
                ? ManifestoDoDestinatario.ItemChoiceType.CPF
                : ManifestoDoDestinatario.ItemChoiceType.CNPJ,

                chNFe = eventoDeManifesto.ChaveDeAcesso,
                dhEvento = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"),

                tpEvento = eventoDeManifesto.TipoDoProcesso.ParseEnumItem<ManifestoDoDestinatario.TEventoInfEventoTpEvento>("tpEvento"),

                nSeqEvento = NUMERO_DE_SEQUENCIA,
                verEvento = "1.00",
                detEvento = ObterDeTalhesDoEventoDeManifesto(eventoDeManifesto),
                Id = ObterIdDoEventoDeManifesto(eventoDeManifesto)
            };

            return infoManifesto;
        }

        private ManifestoDoDestinatario.TEventoInfEventoDetEvento ObterDeTalhesDoEventoDeManifesto(EventoDestinatarioModel eventoDeManifesto)
        {
            var detEvento = new ManifestoDoDestinatario.TEventoInfEventoDetEvento
            {
                descEvento = ObterDescricaoDoEventoDeManifesto(eventoDeManifesto),
                xJust = "",
                versao = ManifestoDoDestinatario.TEventoInfEventoDetEventoVersao.Item100
            };

            return detEvento;
        }


        private ManifestoDoDestinatario.TEventoInfEventoDetEventoDescEvento ObterDescricaoDoEventoDeManifesto(EventoDestinatarioModel eventoDeManifesto)
        {


            return ManifestoDoDestinatario.TEventoInfEventoDetEventoDescEvento.CienciadaOperacao;

        }

        private string ObterIdDoEventoDeManifesto(EventoDestinatarioModel eventoDeManifesto)
        {

            return "ID" +
                   $"{eventoDeManifesto.TipoDoProcesso}" +
                   $"{eventoDeManifesto.ChaveDeAcesso}" +
                   $"01";
        }





        public EventoRetornoModel ConverterRetornoDoManifesto(string xmlDeEnvio, string xmlDoRetorno)
        {
            var retorno = xmlDoRetorno.XmlStringToClass<ManifestoDoDestinatario.TRetEnvEvento>();

            return new EventoRetornoModel
            {
                versao = retorno.versao,
                idDoLote = retorno.idLote,
                tpAmb = (short)retorno.tpAmb,
                verAplic = retorno.verAplic,
                cOrgao = (short)retorno.cOrgao,
                cStat = short.Parse(retorno.cStat),
                xMotivo = retorno.xMotivo,
                XmlEnvio = xmlDeEnvio,
                XmlRetorno = xmlDoRetorno,
                RetornosDosEventos = ObterRetornosDosEventos(retorno.retEvento)
            };
        }

        private List<RetornoEventosModel> ObterRetornosDosEventos(ManifestoDoDestinatario.TretEvento[] retornoRetEvento)
        {
            var retornos = new List<RetornoEventosModel>();

            if (retornoRetEvento == null)
                return retornos;

            retornos.AddRange(retornoRetEvento.Select(ObterRetornoDoEvento));

            return retornos;
        }

        private RetornoEventosModel ObterRetornoDoEvento(ManifestoDoDestinatario.TretEvento retorno)
        {
            return new RetornoEventosModel
            {
                versao = retorno.versao,
                id = retorno.infEvento.Id,
                tpAmb = (short)retorno.infEvento.tpAmb,
                verAplic = retorno.infEvento.verAplic,
                cOrgao = (short)retorno.infEvento.cOrgao,
                cStat = short.Parse(retorno.infEvento.cStat),
                xMotivo = retorno.infEvento.xMotivo,
                chNFe = retorno.infEvento.chNFe,
                tpEvento = int.Parse(retorno.infEvento.tpEvento),
                xEvento = retorno.infEvento.xEvento,
                nSeqEvento = retorno.infEvento.nSeqEvento,
                CNPJDest = retorno.infEvento.ItemElementName == ManifestoDoDestinatario.ItemChoiceType.CNPJ ? retorno.infEvento.Item : null,
                CPFDest = retorno.infEvento.ItemElementName == ManifestoDoDestinatario.ItemChoiceType.CPF ? retorno.infEvento.Item : null,
                emailDest = retorno.infEvento.emailDest,
                dhRegEvento = DateTime.Parse(retorno.infEvento.dhRegEvento),
                nProt = retorno.infEvento.nProt,
            };
        }
    }
}