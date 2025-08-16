
namespace connect4_backend.Services;

public interface ISharedService
{
    public Task<string?> GetUserId(string email);
    public Task MarkMatchAsFinished(string id, string winner);
}