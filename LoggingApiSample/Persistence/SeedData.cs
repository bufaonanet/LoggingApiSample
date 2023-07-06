using LoggingApiSample.Entities;

namespace LoggingApiSample.Persistence;

public class SeedData
{
    private readonly ApiContext _context;

    public SeedData(ApiContext context)
    {
        _context = context;
    }

    public void Initialize()
    {
        _context.Database.EnsureCreated();

        bool atualizarDb = false;

        if (!_context.Indicadores.Any())
        {
            atualizarDb = true;

            _context.Indicadores.AddRange(
            new Indicador
            {
                Sigla = "SALARIO",
                NomeIndicador = "Salario minimo",
                UltimaAtualizacao = new DateTime(2021, 01, 01),
                Valor = 1100.00m
            },
            new Indicador
            {
                Sigla = "IPCA",
                NomeIndicador = "Indice Nacional de Precos ao Consumidor Amplo",
                UltimaAtualizacao = new DateTime(2021, 08, 31),
                Valor = 0.0087m
            },
            new Indicador
            {
                Sigla = "SELIC",
                NomeIndicador = "Indice Nacional de Precos ao Consumidor Amplo",
                UltimaAtualizacao = new DateTime(2021, 09, 22),
                Valor = 0.06257m
            });
            _context.SaveChanges();
        }

        if (!_context.BolsasValores.Any())
        {
            atualizarDb = true;

            _context.BolsasValores.AddRange(
            new BolsaValores
            {
                Sigla = "NASDAQ",
                NomeBolsa = "EUA | NASDAQ",
                DataReferencia = new DateTime(2021, 10, 05),
                Variacao = 0.0140m
            },
            new BolsaValores
            {
                Sigla = "BOVESPA",
                NomeBolsa = "Brasil | Bovespa",
                DataReferencia = new DateTime(2021, 10, 05),
                Variacao = 0.0006m
            },
            new BolsaValores
            {
                Sigla = "NIKKEI",
                NomeBolsa = "Japão | Nikkei",
                DataReferencia = new DateTime(2021, 10, 06),
                Variacao = -0.0105m
            });

        }

        if (atualizarDb) _context.SaveChanges();
    }
}