using CurrencyConvertor.Models;
using System.Threading.Tasks;

public interface IExchangeService
{
    Task<ExchangeRate> GetLatestRatesAsync(string baseCurrency);
    Task<ExchangeRate> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount);
    Task<PagedResult<ExchangeRate>> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate, int page, int pageSize);
}
