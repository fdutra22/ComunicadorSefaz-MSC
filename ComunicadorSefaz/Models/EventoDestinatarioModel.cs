namespace ComunicadorSefaz.Models
{
    public class EventoDestinatarioModel
    {
        public ulong IdLote { get; set; }
        public string TipoDoProcesso { get; set; }
        public short tpAmb { get; set; }
        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public short mod { get; set; }
        public short cUF { get; set; }
        public string versao { get; set; }
        public string versaoSchema { get; set; }
        public string ChaveDeAcesso { get; set; }
    }
}
