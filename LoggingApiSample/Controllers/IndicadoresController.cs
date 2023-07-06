using LoggingApiSample.Entities;
using LoggingApiSample.Persistence;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc;

namespace LoggingApiSample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IndicadoresController : ControllerBase
{
    private readonly ILogger<IndicadoresController> _logger;

    private readonly ApiContext _context;
    private static object syncObject = Guid.NewGuid();
    //private readonly IDistributedCache _cache;
    private readonly TelemetryConfiguration _telemetryConfig;

    public IndicadoresController(
        ILogger<IndicadoresController> logger,
        ApiContext context,
        //IDistributedCache cache,
        TelemetryConfiguration telemetryConfig)
    {
        _context = context;
        _logger = logger;
        //_cache = cache;
        _telemetryConfig = telemetryConfig;
    }

    [HttpGet]
    public IEnumerable<Indicador> Get()
    {
        var indicadores =
            (from i in _context.Indicadores
             select i).ToArray();

        _logger.LogInformation($"No. de Indicadores encontrados = {indicadores.Length}");

        return indicadores;
    }

    [HttpGet("{sigla}")]
    public async Task<ActionResult<Indicador>> Get(string sigla)
    {
        var telemetryClient = new TelemetryClient(_telemetryConfig);

        try
        {
            var indicador = await _context.Indicadores.FindAsync(sigla);

            if (indicador == null)
                throw new Exception($"Simulacao de Falha. Siglas {sigla} não encontradas");

            _logger.LogInformation("Gerando Custom Event do Application Insights...");

            telemetryClient.TrackEvent("Indicadores", GetDictionaryValorAtual(indicador.NomeIndicador));

            return indicador;
        }
        catch (Exception ex)
        {
            _logger.LogError("Excecao - Mensagem: {Message}", ex.Message);
            _logger.LogWarning("Registrando Exception com o Application Insights...");
            telemetryClient.TrackException(ex, GetDictionaryValorAtual(sigla));

            return NotFound();
        }
    }

    private static Dictionary<string, string> GetDictionaryValorAtual(string nomeIndicador) => new()
    {
        { "Horario", DateTime.Now.ToString("HH:mm:ss") },
        { "Indicador", nomeIndicador }
    };

}

