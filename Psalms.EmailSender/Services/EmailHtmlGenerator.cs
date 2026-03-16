namespace Psalms.EmailSender.Services;

public class EmailHtmlGenerator : IEmailHtmlGenerator
{
    public async Task<string> GenerateHtmlAsync(string path, string type, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("O caminho do template não pode ser nulo ou vazio.", nameof(path));

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("O tipo do template não pode ser nulo ou vazio.", nameof(type));

        return await File.ReadAllTextAsync($"{path}/{type}.html", ct);
    }
}

/// <summary>
/// Defines a contract for generating HTML content for emails based on a specified template path and type.
/// </summary>
/// <remarks>Implementations of this interface are responsible for producing HTML suitable for email bodies. The
/// method is asynchronous to support scenarios where template loading or processing may involve I/O operations, such as
/// reading from disk or a remote source.</remarks>
public interface IEmailHtmlGenerator
{
    /// <summary>
    /// Generates HTML content for an email based on the provided template path and type asynchronously.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    Task<string> GenerateHtmlAsync(string path, string type, CancellationToken ct);
}