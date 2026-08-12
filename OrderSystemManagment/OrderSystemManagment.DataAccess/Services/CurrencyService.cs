using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OrderSystemManagment.DataAccess.Configuration;
using OrderSystemManagment.Domain.Interfaces;

namespace OrderSystemManagment.DataAccess.Services;

public class CurrencyService : ICurrencyService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly TcmbSettings _settings;

    public CurrencyService(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        IOptions<TcmbSettings> settings)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _settings = settings.Value;
    }

    public async Task<(string Date, Dictionary<string, decimal> Rates, Dictionary<string, decimal> Units)> GetRatesAsync(DateTime? targetDate = null)
    {
        var requestedDate = targetDate ?? DateTime.Now;
        var date = requestedDate;
        if (date > DateTime.Now)
        {
            date = DateTime.Now;
        }

        if (requestedDate.TimeOfDay != TimeSpan.Zero && requestedDate.TimeOfDay < new TimeSpan(15, 30, 0))
        {
            date = date.AddDays(-1);
        }

        var formattedDate = date.ToString("dd.MM.yyyy");

        var cacheKey = $"TcmbRates_{formattedDate}";
        if (_cache.TryGetValue(cacheKey, out (string Date, Dictionary<string, decimal> Rates, Dictionary<string, decimal> Units) cachedResult))
        {
            return cachedResult;
        }

        var client = _httpClientFactory.CreateClient();
        int maxRetries = 10;
        for (int i = 0; i < maxRetries; i++)
        {
            var currentFormatted = date.ToString("dd.MM.yyyy");
            var innerCacheKey = $"TcmbRates_{currentFormatted}";

            // Check cache for intermediate dates during retro-seek
            if (_cache.TryGetValue(innerCacheKey, out (string Date, Dictionary<string, decimal> Rates, Dictionary<string, decimal> Units) innerCached))
            {
                // Store in the original cacheKey too if we traversed back
                if (cacheKey != innerCacheKey)
                {
                    var originalCacheDuration = requestedDate.Date >= DateTime.Today ? TimeSpan.FromHours(1) : TimeSpan.FromDays(30);
                    _cache.Set(cacheKey, innerCached, originalCacheDuration);
                }
                return innerCached;
            }

            var url = $"{_settings.BaseUrl.TrimEnd('/')}/{date:yyyyMM}/{date:ddMMyyyy}.xml";

            try
            {
                var xmlData = await client.GetStringAsync(url);
                var doc = XDocument.Parse(xmlData);

                var bulletinDate = doc.Root?.Attribute("Tarih")?.Value ?? date.ToString("dd.MM.yyyy");
                var rates = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
                var units = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

                rates["TRY"] = 1.0m;
                rates["TL"] = 1.0m;
                units["TRY"] = 1m;
                units["TL"] = 1m;

                if (doc.Root != null)
                {
                    foreach (var element in doc.Root.Elements("Currency"))
                    {
                        var code = element.Attribute("Kod")?.Value;
                        var forexBuyingStr = element.Element("ForexBuying")?.Value;
                        var unitStr = element.Element("Unit")?.Value;

                        if (!string.IsNullOrWhiteSpace(code) &&
                            !string.IsNullOrWhiteSpace(forexBuyingStr) &&
                            decimal.TryParse(forexBuyingStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var forexBuying))
                        {
                            decimal unit = 1;
                            if (!string.IsNullOrWhiteSpace(unitStr))
                            {
                                decimal.TryParse(unitStr, NumberStyles.Any, CultureInfo.InvariantCulture, out unit);
                            }
                            if (unit > 0)
                            {
                                rates[code] = forexBuying; // stored as raw bulletin rate (e.g. per 100 JPY unit)
                                units[code] = unit;        // original TCMB unit (e.g. 100 for JPY)
                            }
                        }
                    }
                }

                var result = (bulletinDate, rates, units);

                // Cache duration: 1 hour for today, 30 days for historical dates since they are static
                var innerCacheDuration = date.Date == DateTime.Today ? TimeSpan.FromHours(1) : TimeSpan.FromDays(30);

                _cache.Set(innerCacheKey, result, innerCacheDuration);
                _cache.Set($"TcmbRates_{bulletinDate}", result, innerCacheDuration);
                if (cacheKey != innerCacheKey)
                {
                    var originalCacheDuration = requestedDate.Date >= DateTime.Today ? TimeSpan.FromHours(1) : TimeSpan.FromDays(30);
                    _cache.Set(cacheKey, result, originalCacheDuration);
                }

                return result;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                date = date.AddDays(-1);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse rates from TCMB for date {date:dd.MM.yyyy}.", ex);
            }
        }

        throw new InvalidOperationException("Failed to find any available TCMB rates in the last 10 days.");
    }

    public async Task<(double ConvertedAmount, decimal ExchangeRate, string RateDate)> ConvertAndGetRateAsync(
        List<(decimal Amount, string Currency)> items,
        string targetCurrency,
        DateTime? targetDate = null)
    {
        var (rateDate, rates, units) = await GetRatesAsync(targetDate);

        decimal totalInTarget = 0;
        foreach (var item in items)
        {
            var amount = item.Amount;
            var fromCurrency = item.Currency;

            var normalizedFrom = NormalizeCurrencyCode(fromCurrency);
            var normalizedTo = NormalizeCurrencyCode(targetCurrency);

            if (normalizedFrom == normalizedTo)
            {
                totalInTarget += amount;
                continue;
            }

            // Get source rate and unit
            decimal fromRate = 1.0m;
            decimal fromUnit = 1.0m;
            if (normalizedFrom != "TRY")
            {
                if (!rates.TryGetValue(normalizedFrom, out fromRate))
                {
                    throw new InvalidOperationException($"Source currency '{fromCurrency}' rate not found in TCMB bulletin.");
                }
                if (!units.TryGetValue(normalizedFrom, out fromUnit) || fromUnit <= 0)
                {
                    fromUnit = 1.0m;
                }
            }

            // Get target rate and unit
            decimal toRate = 1.0m;
            decimal toUnit = 1.0m;
            if (normalizedTo != "TRY")
            {
                if (!rates.TryGetValue(normalizedTo, out toRate))
                {
                    throw new InvalidOperationException($"Target currency '{targetCurrency}' rate not found in TCMB bulletin.");
                }
                if (!units.TryGetValue(normalizedTo, out toUnit) || toUnit <= 0)
                {
                    toUnit = 1.0m;
                }
            }

            if (toRate == 0)
            {
                throw new InvalidOperationException($"TCMB rate for target currency '{targetCurrency}' is zero.");
            }

            var amountInTry = amount * (fromRate / fromUnit);
            var amountInTarget = amountInTry / (toRate / toUnit);
            totalInTarget += amountInTarget;
        }

        var targetCode = NormalizeCurrencyCode(targetCurrency);
        decimal exchangeRateOfTarget = 1.0m;
        if (rates.TryGetValue(targetCode, out var r))
        {
            exchangeRateOfTarget = r;
        }

        return ((double)totalInTarget, exchangeRateOfTarget, rateDate);
    }

    public async Task<string?> GetRateDisplayNoteAsync(string currencyName, DateTime? targetDate = null)
    {
        var normalizedCode = NormalizeCurrencyCode(currencyName);
        if (normalizedCode.Equals("TRY", StringComparison.OrdinalIgnoreCase))
            return null;

        var (bulletinDate, rates, units) = await GetRatesAsync(targetDate);

        if (!rates.TryGetValue(normalizedCode, out var ratePerUnit))
            return null;

        if (!units.TryGetValue(normalizedCode, out var unit) || unit == 0)
        {
            unit = 1m;
        }

        // ratePerUnit is already the raw bulletin rate (e.g. 28.60 for JPY)
        var displayRate = ratePerUnit;
        var unitDisplay = unit > 1 ? $"{unit:N0} {currencyName}" : $"1 {currencyName}";
        return $"{unitDisplay} = {displayRate:N4} TL (Kur Bülten Tarihi: {bulletinDate})";
    }

    private static readonly IReadOnlyList<(string IsoCode, string[] Keywords)> CurrencyKeywords =
    [
        ("TRY", ["TÜRK", "TURK", "LİRA", "LIRA", "TL", "TRY"]),
        ("USD", ["DOLAR", "USD", "$"]),
        ("EUR", ["EURO", "AVRO", "EUR", "€"]),
        ("GBP", ["STERLIN", "POUND", "İNGİLİZ", "INGILIZ", "GBP"]),
        ("JPY", ["JAPON", "YEN", "JPY"]),
        ("CHF", ["İSVİÇRE", "ISVICRE", "FRANK", "CHF"]),
        ("SEK", ["SEK", "İSVEÇ", "ISVEC"]),
        ("NOK", ["NOK", "NORVEÇ", "NORVEC"]),
        ("DKK", ["DKK", "DANİMARKA", "DANIMARKA"]),
        ("CAD", ["CAD", "KANADA"]),
        ("AUD", ["AUD", "AVUSTRALYA"]),
        ("CNY", ["CNY", "ÇİN", "CIN", "YUAN"]),
        ("SAR", ["SAR", "SUUDI", "RİYAL", "RIYAL"]),
        ("AED", ["AED", "DİRHEM", "DIRHEM", "DUBAI"]),
        ("KWD", ["KWD", "KUVEYT"]),
        ("QAR", ["QAR", "KATAR"]),
        ("BGN", ["BGN", "BULGAR", "LEV"]),
        ("RON", ["RON", "RUMEN", "LEU"]),
        ("HUF", ["HUF", "MACAR", "FORİNT"]),
        ("PLN", ["PLN", "POLONYA", "ZLOT"]),
        ("CZK", ["CZK", "ÇEK", "CEK", "KORUNA"]),
        ("RUB", ["RUB", "RUS", "RUBLE"]),
        ("UAH", ["UAH", "UKRAYNA", "HRYVNIA"]),
        ("IRR", ["IRR", "İRAN", "IRAN"]),
        ("PKR", ["PKR", "PAKİSTAN", "PAKISTAN", "RUPİ"]),
        ("INR", ["INR", "HİNDİSTAN", "HINDISTAN"]),
    ];

    public string NormalizeCurrencyCode(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "TRY";

        var upper = name.Trim().ToUpperInvariant();

        foreach (var (isoCode, keywords) in CurrencyKeywords)
        {
            foreach (var keyword in keywords)
            {
                if (upper == keyword || upper.Contains(keyword))
                    return isoCode;
            }
        }

        return upper;
    }
}