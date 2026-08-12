using OrderSystemManagment.Domain.Entities.BaseModels;
using System.Collections.ObjectModel;

namespace OrderSystemManagment.Domain.Entities;

public class Order : DatabaseObject
{
    
    public Customer? Customer { get; set; }//CustomerName

    public Invoice? Invoice { get; set; }//InvoiceNumber, InvoiceType, InvoiceTotal, 'InvoiceDate'i OrderDate olarak', GrandTotal

    public DateTime PaymentDate { get; set; }

    public CurrencyType? CurrencyType { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new Collection<OrderItem>();
}