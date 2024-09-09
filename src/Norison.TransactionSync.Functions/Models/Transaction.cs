namespace Norison.TransactionSync.Functions.Models;

public class Transaction
{
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public decimal AmountFrom { get; set; }
    public decimal AmountTo { get; set; }
}