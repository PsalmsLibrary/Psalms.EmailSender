namespace Psalms.EmailSender.Models;

/// <summary>
/// Represents the data required to generate or process an email token, including its type and associated command
/// information in JSON format.
/// </summary>
/// <param name="Type">The type of the email token, used to identify the purpose or category of the token. Cannot be null or empty.</param>
/// <param name="CommandJson">A JSON-formatted string containing command or payload data associated with the token. The structure and content
/// depend on the token type. Cannot be null.</param>
public record EmailTokenData(string Type, string CommandJson);

/// <summary>
/// Represents a command with its associated .NET type and serialized JSON payload.
/// </summary>
/// <param name="Type">The .NET type that defines the structure or contract of the command.</param>
/// <param name="JsonCommand">The JSON-formatted string containing the serialized command data. Cannot be null.</param>
public record CommandModel(Type Type, string JsonCommand);