using OrderSystemManagment.Domain.Entities.BaseModels;
using System;
using System.Collections.Generic;

namespace OrderSystemManagment.Domain.Entities;
public class Invoice : DatabaseObject
{

    public string InvoiceNumber { get; set; } = string.Empty;
    
    public string InvoiceType { get; set; } = string.Empty;

    public DateTime InvoiceDate { get; set; }
    
    public Order? Order { get; set; }//OrderId

    public CurrencyType? CurrencyType { get; set; }//CurrencyId

    public double GrandTotal { get; set; }

    public decimal ExchangeRate { get; set; }

    public string ExchangeRateDate { get; set; } = string.Empty;
}
