namespace SuitStore.Alterations.Core.Models;

public class Alteration
{
    public Guid AlterationId { get; set; }
    public long? OrderId { get; set; }
    public long ClientId { get; set; }
    public long ProductId { get; set; }
    public long? TailorId { get; set; }
    public IEnumerable<AlterationInstruction> AlterationInstructions { get; set; } = default!;
    public DateTime CreatedAtDateUtc { get; set; }
    public DateTime? CompletedAtDateUtc { get; set; }
    public string CurrentState { get; set; } = default!;
}