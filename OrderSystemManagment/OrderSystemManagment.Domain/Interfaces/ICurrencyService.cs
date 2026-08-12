using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderSystemManagment.Domain.Interfaces;

public interface ICurrencyService
{
    Task<(string Date, Dictionary<string, decimal> Rates, Dictionary<string, decimal> Units)> GetRatesAsync(DateTime? targetDate = null);
    
    Task<(double ConvertedAmount, decimal ExchangeRate, string RateDate)> ConvertAndGetRateAsync(
        List<(decimal Amount, string Currency)> items, 
        string targetCurrency, 
        DateTime? targetDate = null);
        
    string NormalizeCurrencyCode(string name);
    Task<string?> GetRateDisplayNoteAsync(string currencyName, DateTime? targetDate = null);
}
