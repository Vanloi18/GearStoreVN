namespace GearStore.Domain.Enums;

/// <summary>
/// Phương thức thanh toán
/// </summary>
public enum PaymentMethod
{
    COD = 1,            // Tiền mặt khi nhận hàng
    BankTransfer = 2    // Chuyển khoản ngân hàng
}