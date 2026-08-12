namespace BOMManagement.Domain.Enums;

public enum PurchaseOrderStatus
{
    Draft = 1,      // Taslak - Oluşturuldu
    Approved = 2,   // Onaylandı - Tedarikçiye İletildi
    Delivered = 3,  // Teslim Edildi - Stoka Eklendi
    Cancelled = 4   // İptal Edildi
}