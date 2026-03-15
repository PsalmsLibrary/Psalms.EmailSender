namespace Psalms.EmailSender.Services;

public class EmailHtmlGenerator : IEmailHtmlGenerator
{
    public async Task<string> GenerateHtmlAsync(string path, string type)
        => await File.ReadAllTextAsync($"{path}/{type}.html");
}

public interface IEmailHtmlGenerator
{
    Task<string> GenerateHtmlAsync(string path, string type);
}