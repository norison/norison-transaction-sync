namespace Norison.BankNotionConnector.Functions.Features.AddStatement;

public class AddStatementRequest
{
    public string Type { get; set; } = string.Empty;
    public AddStatementData Data { get; set; } = new();
}