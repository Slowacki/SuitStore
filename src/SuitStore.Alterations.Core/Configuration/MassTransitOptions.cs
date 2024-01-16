using System.ComponentModel.DataAnnotations;

namespace SuitStore.Alterations.Core.Configuration;

public class MassTransitOptions
{
    [Required]
    public string Url { get; set; }
}