using GearStore.Domain.Enums;

namespace GearStore.Application.DTOs.Order;

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}
