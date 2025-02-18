using ComunicadorSefaz.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ComunicadorSefaz.Controllers;

[ApiController]
[Route("[controller]")]
public class ComunicaoSefazController : ControllerBase
{
    private readonly ICienciaEmissaoService _cienciaEmissaoService;

    public ComunicaoSefazController(ICienciaEmissaoService cienciaEmissaoService )
    {
        _cienciaEmissaoService = cienciaEmissaoService;
    }

    [HttpGet]
    public IActionResult Get(string chaveNota)
    {
       var retorno = _cienciaEmissaoService.CienciaEmissaoAsync(chaveNota);

        return Ok(retorno);
    }
}
