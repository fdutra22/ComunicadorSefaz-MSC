using ComunicadorSefaz.Services;
using ComunicadorSefaz.Services.Interfaces;
using Moq;
using System.Security.Cryptography.X509Certificates;

namespace ComunicadorSefaz.Test
{
    public class ComunicacaoSefazServiceTest
    {
        [Fact]
        public async Task NaoRetorna_ExceptionAsync()
        {

            var mockMontarXmlEnvioService = new Mock<IMontarXmlEnvioService>();
            var mockCertificado = new Mock<X509Certificate2>();
            var comunicadorSefazService = new ComunicadorSefazService(mockMontarXmlEnvioService.Object);
            var result = await comunicadorSefazService.EnviarManifestoAsync(mockCertificado.Object, "1234567890");
            Assert.NotNull(result);
        }
    }
}
