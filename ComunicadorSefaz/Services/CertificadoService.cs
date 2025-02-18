using ComunicadorSefaz.Services.Interfaces;
using System.Security.Cryptography.X509Certificates;

namespace ComunicadorSefaz.Services
{
    public class CertificadoService : ICertificadoService
    {
        public async Task<X509Certificate2> CarregarCertificadoMaisAsync(string path = null)
        {
            var certificadoLocal = X509CertificateLoader.LoadCertificateFromFile(path);

            return certificadoLocal;
        }
    }
}
