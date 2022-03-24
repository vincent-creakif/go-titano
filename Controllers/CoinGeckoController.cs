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
        // For the first day of the month, we make sure to get all the history of the previous month
        if (now.Day == 1)
        {
            now = now.AddMonths(-1);
        }
        foreach (var currency in CurrenciesGroups.CurrenciesAvailable)
        {
            var month = new DateTimeOffset(new DateTime(now.Year, now.Month, 1));
            var totalMonths = ((now.Year * 12 + now.Month) - (month.Year * 12 + month.Month)) + 1;
            for (var i = 0; i < totalMonths; i++)
            {
                await _coinGeckoService.GetMonthlyMarketChartAsync(Coins.Titano, currency, month, ct);
                month = month.AddMonths(1);
            }
        }
        
        return Ok();
    }
}
