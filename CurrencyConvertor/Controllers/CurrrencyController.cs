using CurrencyConvertor.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly IExchangeService _exchangeService;

    public CurrencyController(IExchangeService exchangeService)
    {
        _exchangeService = exchangeService;
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency)
    {
        try
        {
            var rates = await _exchangeService.GetLatestRatesAsync(baseCurrency);
            return Ok(rates);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("convert")]
    public async Task<IActionResult> ConvertCurrency([FromBody] ConversionRequest request)
    {
        try
        {
            var result = await _exchangeService.ConvertCurrencyAsync(request.FromCurrency, request.ToCurrency, request.Amount);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("history")]
    public async Task<IActionResult> GetHistoricalRates([FromQuery] string baseCurrency, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var rates = await _exchangeService.GetHistoricalRatesAsync(baseCurrency, startDate, endDate, page, pageSize);
            return Ok(rates);
        }
        catch (ApiException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
