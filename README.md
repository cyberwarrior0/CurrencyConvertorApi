# Currency Converter API

## Overview

This is a simple currency converter API built with ASP.NET Core. The API provides endpoints to get the latest exchange rates, convert currencies, and retrieve historical exchange rates.

## Endpoints

### Get Latest Rates

**Endpoint:** `GET /api/currency/latest`

**Query Parameters:**
- `baseCurrency`: The base currency to get the latest exchange rates for.

**Response:**
Returns the latest exchange rates for the specified base currency.
json
{
    "base": "EUR",
    "rates": {
        "USD": 1.1,
        "GBP": 0.9
    },
    "date": "2023-05-19"
}


### Convert Currency

**Endpoint: POST /api/currency/convert**

**Request Body:**
{
    "fromCurrency": "EUR",
    "toCurrency": "USD",
    "amount": 100
}
**Response:**
Returns the converted amount in the target currency.
{
    "base": "EUR",
    "date": "2023-05-19",
    "rates": {
        "USD": 110
    }
}


### Get Historical Rates

**Endpoint: GET /api/currency/history**

**Query Parameters:**

baseCurrency: The base currency to get historical rates for.
startDate: The start date for the historical data (YYYY-MM-DD).
endDate: The end date for the historical data (YYYY-MM-DD).
page: The page number (default is 1).
pageSize: The number of items per page (default is 10).

**Response:**
Returns paged historical exchange rates for the specified base currency and date range.
{
    "currentPage": 1,
    "pageSize": 10,
    "totalItems": 20,
    "items": [{
            "base": "EUR",
            "date": "2023-05-18",
            "rates": {
                "USD": 1.1
            }
        }
    ]
}


### Rate Limiting Middleware
The API includes a rate limiting middleware to prevent clients from making too many requests in a short period. The rate limiting is set to allow one request every five seconds per client IP address.

**Implementation**
The rate limiting middleware is implemented using the RateLimitingMiddleware class


### Handling API Retries
In some cases, the Frankfurter API may not respond to the first request but to the second or third. This API handles such cases by implementing a retry mechanism with a maximum of 3 attempts.

**Implementation**
The retry mechanism is implemented in the ExecuteWithRetryAsync method of the ExchangeService class


### Testing
Added Unit test cases.
To run the unit tests for the API:
1. Go the project path in the git bash
2. Run "dotnet test" in the bash


### Conclusion
This API provides a simple and effective way to get the latest exchange rates, convert currencies, and retrieve historical exchange rates. 
The rate limiting middleware helps to protect the API from excessive requests, ensuring fair usage for all clients. 
Additionally, the retry mechanism ensures reliability when fetching data from the Frankfurter API.

Unit tests are implemented to verify the functionality and robustness of the API endpoints.


