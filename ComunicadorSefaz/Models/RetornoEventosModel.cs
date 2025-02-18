namespace ComunicadorSefaz.Models
{
    public class RetornoEventosModel
    {
        public string versao { get; set; }
        public string id { get; set; }
        public short tpAmb { get; set; }
        public string verAplic { get; set; }
        public short cOrgao { get; set; }
        public short cStat { get; set; }
        public string xMotivo { get; set; }
        public string chNFe { get; set; }
        public int tpEvento { get; set; }
        public string xEvento { get; set; }
        public string nSeqEvento { get; set; }
        public string CNPJDest { get; set; }
        public string CPFDest { get; set; }
        public string emailDest { get; set; }
        public DateTime dhRegEvento { get; set; }
        public string nProt { get; set; }
        public string[] chNFePend { get; set; }
        public int cOrgaoAutor { get; set; }
        public string idLote { get; set; }
    }
}
