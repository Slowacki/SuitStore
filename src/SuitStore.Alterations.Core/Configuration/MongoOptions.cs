using System.ComponentModel.DataAnnotations;

namespace SuitStore.Alterations.Core.Configuration;

public class MongoOptions
{
    [Required] public string ConnectionString { get; set; } = default!;

    [Required]
    public string DatabaseName { get; set; } = default!;
}