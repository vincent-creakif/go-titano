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
        await _coinGeckoService.GetMonthlyMarketChartAsync(Coins.Titano, ct);
        
        return Ok();
    }
}
