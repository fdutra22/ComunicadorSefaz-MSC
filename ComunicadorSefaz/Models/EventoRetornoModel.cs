namespace ComunicadorSefaz.Models
{
    public class EventoRetornoModel
    {
        public List<RetornoEventosModel> RetornosDosEventos { get; set; }
        public string versao { get; set; }
        public string idDoLote { get; set; }
        public short tpAmb { get; set; }
        public short cUF { get; set; }
        public short cOrgao { get; set; }
        public string verAplic { get; set; }
        public short cStat { get; set; }
        public string xMotivo { get; set; }
        public DateTime dhRecbto { get; set; }
        public string nRec { get; set; }
        public short cMsg { get; set; }
        public string xMsg { get; set; }
        public string XmlEnvio { get; set; }
        public string XmlRetorno { get; set; }
        public string url { get; set; }
    }
}
