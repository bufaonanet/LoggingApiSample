using LoggingApiSample.Entities;
using LoggingApiSample.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace LoggingApiSample.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BolsasController : ControllerBase
{
    private readonly Serilog.ILogger _logger;

    private readonly ApiContext _context;

    public BolsasController(ApiContext context)
    {
        _context = context;
        _logger = Serilog.Log.ForContext<BolsasController>();
    }

    [HttpGet]
    public IEnumerable<BolsaValores> Get()
    {
        var indicadoresBolsasValores =
            (from b in _context.BolsasValores
             select b).ToArray();

        _logger.Information($"No. de Indicadores de Bolsas de Valores encontrados = {indicadoresBolsasValores.Length}");

        return indicadoresBolsasValores;
    }
}