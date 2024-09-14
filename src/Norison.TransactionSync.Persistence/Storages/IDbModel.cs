namespace Norison.TransactionSync.Persistence.Storages;

public interface IDbModel
{
    public string? Id { get; set; }
    public string? IconUrl { get; set; }
}