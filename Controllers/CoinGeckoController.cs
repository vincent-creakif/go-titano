using Microsoft.AspNetCore.Mvc;

namespace Creakif.GoTitano.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoinGeckoController : ControllerBase
{
    private readonly CoinGeckoService _coinGeckoService;

    public CoinGeckoController(CoinGeckoService coinGeckoService)
    {
        _coinGeckoService = coinGeckoService;
    }

    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResult))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [HttpGet("MonthlyMarketChart")]
    public async Task<ActionResult> GetMonthlyMarketChartAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        foreach (var currency in CurrenciesGroups.CurrenciesAvailable)
        {
            var month = new DateTimeOffset(new DateTime(2021, 11, 1));

            var totalMonths = (now.Year * 12 + now.Month) - (month.Year * 12 + month.Month);
            for (var i = 0; i < totalMonths; i++)
            {
                await _coinGeckoService.GetMonthlyMarketChartAsync(Coins.Titano, currency, month, ct);
                month = month.AddMonths(1);
            }
        }
        
        return Ok();
    }
}
