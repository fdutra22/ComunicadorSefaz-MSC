using System.Security.Cryptography.X509Certificates;

namespace ComunicadorSefaz.Services.Interfaces
{
    public interface IComunicadorSefazService
    {

        public Task<dynamic> EnviarManifestoAsync(X509Certificate2 certificado, string chaveNota);

    }
}