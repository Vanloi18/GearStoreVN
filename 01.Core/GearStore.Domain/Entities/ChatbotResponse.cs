using GearStore.Domain.Common;

namespace GearStore.Domain.Entities;

public class ChatbotResponse : BaseEntity
{
    public string Keywords { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string MatchType { get; set; } = "Contains"; 
    public int Priority { get; set; } = 0;
}
