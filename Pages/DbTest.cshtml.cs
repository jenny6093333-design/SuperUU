using Microsoft.AspNetCore.Mvc.RazorPages;
using SuperUU.Data;

namespace SuperUU.Pages;

public class DbTestModel : PageModel
{
    private readonly AppDbContext _db;

    public DbTestModel(AppDbContext db)
    {
        _db = db;
    }

    public string ConnectionStatus { get; private set; } = "Unknown";

    public async Task OnGetAsync()
    {
        var canConnect = await _db.Database.CanConnectAsync();
        ConnectionStatus = canConnect ? "OK" : "Failed";
    }
}
