using SuitStore.Email.Messaging.Models;

namespace SuitStore.Email.Messaging.Commands;

public record SendEmail(long ClientId, EmailType Type);