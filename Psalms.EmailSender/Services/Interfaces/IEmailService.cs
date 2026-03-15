using Psalms.EmailSender.Models;

namespace Psalms.EmailSender.Services.Interfaces;

/// <summary>
/// Defines methods for sending emails and handling email confirmation workflows asynchronously.
/// </summary>
/// <remarks>Implementations of this interface provide functionality for sending general emails, initiating email
/// confirmation processes, and confirming email addresses using tokens. Methods are asynchronous and intended for use
/// in scenarios where email operations may involve network I/O or require user interaction.</remarks>
public interface IEmailService
{
    /// <summary>
    /// Confirma a operação associada ao token fornecido e executa a ação correspondente de forma assíncrona.
    /// </summary>
    /// <param name="token">O token de confirmação que identifica de forma exclusiva a operação a ser confirmada e executada. Não pode ser
    /// nulo ou vazio.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona de confirmação e execução.</returns>
    Task ConfirmAndExecuteAsync(string token, CancellationToken ct);
    /// <summary>
    /// Confirma o endereço de e-mail de um usuário com base no token fornecido.
    /// </summary>
    /// <param name="token">O token de confirmação de e-mail recebido pelo usuário. Não pode ser nulo ou vazio.</param>
    /// <returns>Um objeto que representa o resultado da confirmação do e-mail, incluindo informações sobre o sucesso ou falha da
    /// operação.</returns>
    Task<CommandModel> ConfirmEmailAsync(string token, CancellationToken ct);
    /// <summary>
    /// Envia uma mensagem de e-mail assíncrona com o assunto e conteúdo HTML especificados para o destinatário
    /// informado.
    /// </summary>
    /// <param name="to">O endereço de e-mail do destinatário. Não pode ser nulo ou vazio.</param>
    /// <param name="subject">O assunto da mensagem de e-mail. Não pode ser nulo ou vazio.</param>
    /// <param name="html">O conteúdo da mensagem em formato HTML. Não pode ser nulo.</param>
    /// <returns>Uma tarefa que representa a operação de envio do e-mail.</returns>
    Task SendAsync(string to, string subject, string html, CancellationToken ct);
    /// <summary>
    /// Sends an email containing a confirmation link to the specified recipient asynchronously.
    /// </summary>
    /// <param name="values">The values required to generate and send the email confirmation, including recipient information and
    /// confirmation details. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendEmailConfirmationAsync(EmailConfirmationValues values, CancellationToken ct);
}