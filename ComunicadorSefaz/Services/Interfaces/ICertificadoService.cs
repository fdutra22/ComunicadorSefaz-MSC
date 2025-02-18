using System.Security.Cryptography.X509Certificates;

namespace ComunicadorSefaz.Services.Interfaces
{
    public interface ICertificadoService
    {
        public Task<X509Certificate2> CarregarCertificadoMaisAsync(string path = null);
    }
}
