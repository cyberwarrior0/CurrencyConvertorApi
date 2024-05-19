using CurrencyConvertor.Models;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

public class ExchangeService : IExchangeService
{
    private const string BaseUrl = "https://api.frankfurter.app/";
    private readonly RestClient _client;
    private readonly ILogger<ExchangeService> _logger;
    private readonly IMemoryCache _cache;
    private const int MaxRetries = 3;
    private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public ExchangeService(ILogger<ExchangeService> logger, IMemoryCache cache)
    {
        _client = new RestClient(new RestClientOptions(BaseUrl));
        _logger = logger;
        _cache = cache;
    }

    private async Task<RestResponse<T>> ExecuteWithRetryAsync<T>(RestRequest request)
    {
        int retries = 0;
        while (retries < MaxRetries)
        {
            var response = await _client.ExecuteAsync<T>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response;
            }
            retries++;
            _logger.LogWarning("Attempt {Attempt} to fetch data failed.", retries);
        }
        throw new ApiException("Error fetching data after multiple attempts.");
    }

    public async Task<ExchangeRate> GetLatestRatesAsync(string baseCurrency)
    {
        var cacheKey = $"latest_{baseCurrency}";
        if (!_cache.TryGetValue(cacheKey, out ExchangeRate rates))
        {
            var request = new RestRequest($"latest?from={baseCurrency}", Method.Get);
            var response = await ExecuteWithRetryAsync<ExchangeRate>(request);
            rates = response.Data;
            _cache.Set(cacheKey, rates, CacheDuration);
        }
        return rates;
    }

    public async Task<ExchangeRate> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount)
    {
        if (new[] { "TRY", "PLN", "THB", "MXN" }.Contains(toCurrency))
        {
            throw new ArgumentException("Conversion to the specified currency is not allowed.");
        }

        var cacheKey = $"convert_{fromCurrency}_{toCurrency}_{amount}";
        if (!_cache.TryGetValue(cacheKey, out ExchangeRate conversion))
        {
            var request = new RestRequest($"latest?amount={amount}&from={fromCurrency}&to={toCurrency}", Method.Get);
            var response = await ExecuteWithRetryAsync<ExchangeRate>(request);
            conversion = response.Data;
            _cache.Set(cacheKey, conversion, CacheDuration);
        }
        return conversion;
    }

    public async Task<PagedResult<ExchangeRate>> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate, int page, int pageSize)
    {
        var cacheKey = $"history_{baseCurrency}_{startDate:yyyy-MM-dd}_{endDate:yyyy-MM-dd}";
        if (!_cache.TryGetValue(cacheKey, out ExchangeRate allRates))
        {
            var request = new RestRequest($"{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}?from={baseCurrency}", Method.Get);
            var response = await ExecuteWithRetryAsync<ExchangeRate>(request);
            allRates = response.Data;
            _cache.Set(cacheKey, allRates, CacheDuration);
        }

        var pagedRates = allRates.Rates
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToDictionary(rate => rate.Key, rate => rate.Value);

        var result = new ExchangeRate
        {
            Base = allRates.Base,
            Rates = pagedRates,
            Date = allRates.Date
        };

        return new PagedResult<ExchangeRate>
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = allRates.Rates.Count,
            Items = result
        };
    }
}
