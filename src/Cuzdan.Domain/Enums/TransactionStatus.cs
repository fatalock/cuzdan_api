namespace Cuzdan.Domain.Enums;
public enum TransactionStatus
{
    Pending,       // İşlem başlatıldı, beklemede
    Completed,     // Başarılı
    Failed,        // Başarısız
    Cancelled,     // İptal edildi
    Processing,    // İşleniyor
    Reversed,      // İade / geri alındı
    Expired,       // Süresi doldu
    OnHold         // İnceleme / blokaj
}