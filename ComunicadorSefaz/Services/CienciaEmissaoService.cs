using ComunicadorSefaz.Services.Interfaces;

namespace ComunicadorSefaz.Services
{
    public class CienciaEmissaoService : ICienciaEmissaoService
    {
        private readonly ILogger<CienciaEmissaoService> _log;
        private readonly ICertificadoService _certificadoService;
        private readonly IComunicadorSefazService _servicoDeComunicacao;


        public CienciaEmissaoService(ILogger<CienciaEmissaoService> log, ICertificadoService certificadoService, IComunicadorSefazService servicoDeComunicacao)
        {
            _log = log;
            _certificadoService = certificadoService;
            _servicoDeComunicacao = servicoDeComunicacao;
        }


        public async Task CienciaEmissaoAsync(string chaveNota)
        {

            try
            {

                var certificado = await _certificadoService.CarregarCertificadoMaisAsync("C:\\path\\certificado.pfx");

                var retornoDoManifesto = await _servicoDeComunicacao.EnviarManifestoAsync(certificado, chaveNota);

                if (retornoDoManifesto.RetornosDosEventos.Count < 1)
                {            
                    _log.LogError($"Retorno do Evento.\n {retornoDoManifesto.xMotivo} ");
                    return;
                }

                var retornoEvento = retornoDoManifesto.RetornosDosEventos[0];

                switch (retornoEvento.cStat)
                {
                    case 573:
                    case 135:
                    case 655:

                        _log.LogError($"Retorno do Evento.\n {retornoEvento.xMotivo} ");

                        return;

                    case 650:
                        
                        _log.LogError($"Retorno do Evento.\n {retornoEvento.xMotivo} ");
                        return;

                    case 596:

                        _log.LogError($"Retorno do Evento.\n {retornoEvento.xMotivo} ");
                        return;
                }

                _log.LogError($"Retorno do Evento.\n {retornoEvento.xMotivo} ");

            }
            catch (Exception exception)
            {
                _log.LogError($"Erro no serviço de ciência da operação.\n{exception.Message}", exception);

                throw new Exception($"Erro no serviço de ciência da operação.\n{exception.Message}", exception);
            }
        }
    }
}
