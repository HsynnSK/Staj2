using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderSystemManagment.Domain.Entities
{
    public static class DomainExtensions
    {
        public static bool UsesNoDecimalPlaces(this CurrencyType? currencyType)
        {
            if (currencyType == null || string.IsNullOrWhiteSpace(currencyType.Name))
                return false;

            return UsesNoDecimalPlaces(currencyType.Name);
        }

        public static bool UsesNoDecimalPlaces(string currencyName)
        {
            if (string.IsNullOrWhiteSpace(currencyName))
                return false;

            return currencyName.Contains("JPY", StringComparison.OrdinalIgnoreCase)
                || currencyName.Contains("Yen", StringComparison.OrdinalIgnoreCase)
                || currencyName.Contains("Japon", StringComparison.OrdinalIgnoreCase)
                || currencyName.Contains("Yeni", StringComparison.OrdinalIgnoreCase)
                || currencyName.Contains("日本円", StringComparison.OrdinalIgnoreCase);
        }

        public static string FormatMoney(this decimal amount, CurrencyType? currencyType)
        {
            var currencyName = currencyType?.Name ?? string.Empty;
            var format = currencyType.UsesNoDecimalPlaces() ? "N0" : "N2";

            return string.IsNullOrWhiteSpace(currencyName)
                ? amount.ToString(format)
                : $"{amount.ToString(format)} {currencyName}";
        }

        public static string FormatMoney(this decimal amount, string currencyName)
        {
            var format = UsesNoDecimalPlaces(currencyName) ? "N0" : "N2";

            return string.IsNullOrWhiteSpace(currencyName)
                ? amount.ToString(format)
                : $"{amount.ToString(format)} {currencyName}";
        }

        public static decimal GetLineTotal(this OrderItem orderItem)
        {
            if (orderItem == null) return 0;
            return orderItem.UnitPrice * orderItem.Quantity;
        }

        public static string FormatLineTotal(this OrderItem orderItem)
        {
            if (orderItem == null) return string.Empty;
            return orderItem.GetLineTotal().FormatMoney(orderItem.CurrencyType);
        }

        public static string BuildGrandTotalSummary(this IEnumerable<OrderItem> orderItems)
        {
            if (orderItems == null) return string.Empty;

            var lineSummaries = orderItems
                .Where(x => x.CurrencyType is not null)
                .GroupBy(x => x.CurrencyType!.Name)
                .Select(group =>
                {
                    var sum = group.Sum(x => x.GetLineTotal());
                    var currencyType = group.First().CurrencyType;
                    return sum.FormatMoney(currencyType);
                })
                .ToList();

            return lineSummaries.Count == 0 ? string.Empty : string.Join(" + ", lineSummaries);
        }
    }
}
