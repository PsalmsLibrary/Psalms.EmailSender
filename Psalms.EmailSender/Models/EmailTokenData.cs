namespace Psalms.EmailSender.Models;

public record EmailTokenData(string Type, string CommandJson);
public record CommandModel(Type Type, string JsonCommand);