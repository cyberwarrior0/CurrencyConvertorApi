using CurrencyConvertor.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class CurrencyControllerTests
{
    private readonly Mock<IExchangeService> _exchangeServiceMock;
    private readonly CurrencyController _controller;

    public CurrencyControllerTests()
    {
        _exchangeServiceMock = new Mock<IExchangeService>();
        _controller = new CurrencyController(_exchangeServiceMock.Object);
    }

    [Fact]
    public async Task GetLatestRates_ReturnsOkResult_WithRates()
    {

        var expectedRates = new ExchangeRate { Base = "EUR", Rates = new Dictionary<string, decimal> { { "USD", 1.1m } } };
        _exchangeServiceMock.Setup(service => service.GetLatestRatesAsync("EUR")).ReturnsAsync(expectedRates);
        var result = await _controller.GetLatestRates("EUR");
        var okResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsType<ExchangeRate>(okResult.Value);
        Assert.Equal("EUR", model.Base);
        Assert.Equal(1.1m, model.Rates["USD"]);
    }

    [Fact]
    public async Task ConvertCurrency_ReturnsOkResult_WithConvertedAmount()
    {

        var request = new ConversionRequest { FromCurrency = "EUR", ToCurrency = "USD", Amount = 100 };
        var expectedConversion = new ExchangeRate { Base = "EUR", Rates = new Dictionary<string, decimal> { { "USD", 110 } } };
        _exchangeServiceMock.Setup(service => service.ConvertCurrencyAsync("EUR", "USD", 100)).ReturnsAsync(expectedConversion);
        var result = await _controller.ConvertCurrency(request);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsType<ExchangeRate>(okResult.Value);
        Assert.Equal("EUR", model.Base);
        Assert.Equal(110, model.Rates["USD"]);
    }

    [Fact]
    public async Task GetHistoricalRates_ReturnsOkResult_WithRates()
    {

        var expectedRates = new ExchangeRate { Base = "EUR", Rates = new Dictionary<string, decimal> { { "USD", 1.1m } } };
        var pagedResult = new PagedResult<ExchangeRate> { Items = expectedRates, TotalItems = 1, CurrentPage = 1, PageSize = 10 };
        _exchangeServiceMock.Setup(service => service.GetHistoricalRatesAsync("EUR", It.IsAny<DateTime>(), It.IsAny<DateTime>(), 1, 10)).ReturnsAsync(pagedResult);
        var result = await _controller.GetHistoricalRates("EUR", DateTime.UtcNow, DateTime.UtcNow, 1, 10);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsType<PagedResult<ExchangeRate>>(okResult.Value);
        Assert.Equal("EUR", model.Items.Base);
        Assert.Equal(1.1m, model.Items.Rates["USD"]);
    }
}
