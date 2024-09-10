using Norison.MonoSdk.Models;

namespace Norison.BankNotionConnector.Functions.Features.AddStatement;

public class AddStatementData
{
    public string Account { get; set; } = string.Empty;
    public Statement StatementItem { get; set; } = new();
}