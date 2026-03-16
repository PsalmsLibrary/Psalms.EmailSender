[Tasks](https://www.notion.so/1f7d41ce36a180b39165e2740a3417ea?pvs=21)

[Resources](https://www.notion.so/Resources-1f7d41ce36a180f49072e0d9a77a068d?pvs=21)

# Psalms.AspNetCore.EmailSender

**Psalms.EmailSender** is a .NET library designed to implement **email confirmation workflows for commands**.

It allows sensitive operations — such as user registration, password resets, or other important actions — to **only be executed after the user confirms the action via email**.

The library integrates naturally with:

- MediatR
- `IDistributedCache`
- MailKit
- MimeKit
- ASP.NET Core Dependency Injection

The main idea is simple: **a command is stored temporarily and executed only after the user confirms the action through an email link**.

---

# Features

- Token-based email confirmation
- Execution of commands after confirmation
- Native integration with **MediatR**
- Support for **distributed cache**
- HTML email templates
- Automatic template resolution per command
- Fully asynchronous
- Clean and extensible architecture

---

# How It Works

Instead of executing a command immediately:

```csharp
await mediator.Send(newRegisterUserCommand(...));
```

The command is wrapped in a **confirmation email workflow**.

### Flow

```
User performs an action
        │
        ▼
SendConfirmationEmailCommand
        │
        ▼
Confirmation token is generated
        │
        ▼
Email with confirmation link is sent
        │
        ▼
User clicks the link
        │
        ▼
ConfirmEmailCommand
        │
        ▼
Original command is restored
        │
        ▼
Command is executed via MediatR
```

This allows **any command to require email confirmation before execution**.

---

# Installation

```
dotnet add package Psalms.AspNetCore.EmailSender
```

---

# Configuration

Add the required configuration to `appsettings.json`.

```json
{
  "Admin": {
    "Email":"admin@email.com"
  },
  "Email": {
    "Host":"smtp.gmail.com",
    "Port":"587",
    "Password":"your-password"
  },
  "FRONTEND_URL":"https://your-frontend.com"
}
```

---

# Dependency Injection

Register the library in `Program.cs`.

```csharp
builder.Services.AddPsalmsEmailSender();
```

This registers:

- `IEmailService`
- `IEmailHtmlGenerator`
- `IEmailTemplateResolver`
- MediatR handlers from the library

---

# Email Templates

Emails are generated from **HTML templates**.

Example:

```
Templates/confirm-email.html
```

Example template:

```html
<h1>Confirm your email</h1>

<p>Click the link below:</p>

<a href="{confirmationLink}">
Confirm Email
</a>
```

The placeholder

```
{confirmationLink}
```

is automatically replaced with the generated confirmation link.

---

# Template Resolver System

One of the core parts of the library is the **Template Resolver**.

Instead of hardcoding templates, the system dynamically resolves the correct email template based on the **command type**.

Internally, the library uses dependency injection to locate the appropriate template.

Simplified logic:

```csharp
vartemplateType=typeof(IEmailTemplate<>).MakeGenericType(commandType);
vartemplate=serviceProvider.GetService(templateType);
```

This means that for every command requiring confirmation, you can create a corresponding:

```csharp
IEmailTemplate<TCommand>
```

---

# Creating an Email Template

Example for a registration command.

```csharp
publicclassRegisterUserEmailTemplate
    :IEmailTemplate<RegisterUserCommand>
{
public string TemplatePath=> "Templates";

public string EmailType=> "confirm-email"; // This should be the .html file name.
// The HTML generator reads it like this: $"{TemplatePath}/{EmailType}.html"

public string Subject=> "Confirm your account";

public string FrontEndPage=> "confirm-email";
}
```

Register it in the dependency injection container:

```csharp
services.AddScoped<IEmailTemplate<RegisterUserCommand>, RegisterUserEmailTemplate>();
```

When `RegisterUserCommand` is sent, this template will automatically be used. 

---

# Sending a Confirmation Email

Instead of executing a command directly:

```csharp
await mediator.Send(newSendConfirmationEmailCommand(command, email));
```

Where:

- `command` → command that should run after confirmation
- `email` → recipient email

---

# Confirming the Email

When the user clicks the confirmation link, the frontend should send the token to the API.

```csharp
await mediator.Send(newConfirmEmailCommand(token));
```

The library will:

1. Validate the token
2. Retrieve the stored command
3. Deserialize it
4. Execute it through **MediatR**

---

# Token Behavior

When an email is sent:

1. A unique token is generated

```
email-confirmation:{guid}
```

1. The command is serialized to JSON
2. The data is stored in `IDistributedCache`

After confirmation:

- the token is removed
- the command is executed
- the token cannot be reused

---

# Token Expiration

Default expiration time:

```
5 minutes
```

You can override it:

```csharp
new EmailConfirmationValues(command, email, templateConfig, TimeSpan.FromMinutes(10));
```

---

## Sending Emails from HTML Templates

The library also provides a simple way to send emails based on **HTML templates stored on disk**. This is handled by the `SendEmailCommand`, the `SendEmailHandler`, and the `IEmailHtmlGenerator`.

This mechanism allows your application to **load an HTML template, generate the email body, and send the email** through the configured email service.

---

### How It Works

When a `SendEmailCommand` is sent through MediatR, the `SendEmailHandler` performs two steps:

1. Generate the HTML content from a template.
2. Send the email using the configured email service.

```csharp
public class SendEmailHandler(
    IEmailService service,
    IEmailHtmlGenerator htmlGenerator
) : IRequestHandler<SendEmailCommand>
{
    public async Task Handle(
        SendEmailCommand request,
        CancellationToken cancellationToken)
    {
        var html = await htmlGenerator.GenerateHtmlAsync(
            request.HtmlPath,
            request.EmailType,
            cancellationToken
        );

        await service.SendAsync(
            request.To,
            request.Subject,
            html,
            cancellationToken
        );
    }
}
```

---

### HTML Template Generator

The `EmailHtmlGenerator` is responsible for loading the HTML template file from disk.

```csharp
public class EmailHtmlGenerator : IEmailHtmlGenerator
{
    public async Task<string> GenerateHtmlAsync(
        string path,
        string type,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException(
                "Template path cannot be null or empty.",
                nameof(path));

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException(
                "Template type cannot be null or empty.",
                nameof(type));

        return await File.ReadAllTextAsync(
            $"{path}/{type}.html",
            ct);
    }
}
```

The generator simply loads the template using the following pattern:

```
{path}/{type}.html
```

Example:

```
Templates/welcome.html
Templates/reset-password.html
Templates/confirmation.html
```

---

### Interface Contract

The generator follows the `IEmailHtmlGenerator` contract:

```csharp
public interface IEmailHtmlGenerator
{
		Task<string> GenerateHtmlAsync(string path, string type, CancellationToken ct);
}
```

This abstraction allows you to replace the default implementation if needed.

For example, you could create generators that load templates from:

- a database
- cloud storage
- an embedded resource
- a template engine

---

### Example Usage

Sending an email using a template:

```csharp
await mediator.Send(
    new SendEmailCommand(
        to: "user@email.com",
        subject: "Welcome",
        htmlPath: "Templates",
        emailType: "welcome"
    )
);
```

The generator will load:

```
Templates/welcome.html
```

The file contents will be used as the email body and sent through the configured email service.

---

### Example Template

```html
<h1>Welcome!</h1>

<p>Your account has been successfully created.</p>

<p>Thank you for joining our platform.</p>
```

---

### Benefits

This approach provides several advantages:

- Keeps email layout separate from application logic
- Makes templates easy to modify
- Supports multiple email types
- Keeps handlers clean and focused
- Allows template loading strategies to be swapped easily

---

# Complete API Example

Below is a minimal **ASP.NET Core API example** using the library.

---

## 1 — Command

```csharp
public record RegisterUserCommand(string Email, string Password) : IRequest;
```

---

## 2 — Command Handler

```csharp
public class RegisterUserHandler : IRequestHandler<RegisterUserCommand>
{
	public async Task Handle(RegisterUserCommand request, CancellationToken ct)
	    {
				// Save user to database
				Console.WriteLine($"User {request.Email} registered.");
	    }
}
```

---

## 3 — Email Template

```csharp
publicclassRegisterUserEmailTemplate : IEmailTemplate<RegisterUserCommand>
{
	public string TemplatePath=>"Templates";
	
	public string EmailType=>"confirm-email";
	
	public string Subject=>"Confirm your account";
	
	public string FrontEndPage=>"confirm";
}
```

---

## 4 — Register Services

```csharp
builder.Services.AddPsalmsEmailSender();

builder.Services.AddScoped<
IEmailTemplate<RegisterUserCommand>,
RegisterUserEmailTemplate>();
```

---

## 5 — Registration Endpoint

```csharp
app.MapPost("/register", async (RegisterUserRequest request, IMediator mediator) =>
{
		var command= newRegisterUserCommand(request.Email, request.Password);
	
		await mediator.Send(new SendConfirmationEmailCommand(command, request.Email));
	
		return Results.Ok("Confirmation email sent.");
});
```

---

## 6 — Email Confirmation Endpoint

```
app.MapPost("/confirm-email",async (
stringtoken,
IMediatormediator)=>
{
awaitmediator.Send(newConfirmEmailCommand(token));

returnResults.Ok("Email confirmed.");
});
```

---

# Use Cases

The library can be used for:

- Email confirmation
- Password reset
- Email change confirmation
- Approval workflows
- Sensitive action verification