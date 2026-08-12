namespace BOMManagement.Domain.Enums;

public enum WorkOrderStatus
{
    Draft = 1,      // Taslak - Planlanıyor
    Released = 2,   // Serbest Bırakıldı - Üretimde
    Completed = 3,  // Tamamlandı - Stoklar Güncellendi
    Cancelled = 4   // İptal Edildi
}
