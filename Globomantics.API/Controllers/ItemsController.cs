using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace SampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    const string PRODUCTION_PASSWORD = "myP2ssword!";

    private static readonly List<Item> _items = new()
    {
        new Item { Id = 1, Name = "Widget", Quantity = 10 },
        new Item { Id = 2, Name = "Gadget", Quantity = 5 }
    };

    [HttpGet]
    public ActionResult<IEnumerable<Item>> Get()
    {
        return Ok(_items);
    }

    [HttpGet("test-mi-connection")]
    public async Task<IActionResult> TestManagedIdentityConnection()
    {
        // Replace with your actual server + database
        const string connectionString =
            "Server=tcp:sql-globomantics-38d31a.database.windows.net,1433;" +
            "Database=GlobomanticsDb;" +
            "Encrypt=True;" +
            "TrustServerCertificate=False;" +
            "Connection Timeout=30;";

        try
        {
            // DefaultAzureCredential will use the system-assigned (or user-assigned) 
            // Managed Identity when running in Azure App Service / Container Apps / etc.
            var credential = new DefaultAzureCredential();

            var token = await credential.GetTokenAsync(
                new TokenRequestContext(new[] { "https://database.windows.net/.default" }));

            await using var connection = new SqlConnection(connectionString);
            connection.AccessToken = token.Token;

            await connection.OpenAsync();   // <-- this is the only thing that matters for the demo

            return Ok("Successfully connected to SQL Database using Managed Identity.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Connection failed",
                error = ex.Message,
                type = ex.GetType().Name
            });
        }
    }


    [HttpGet("{id}")]
    public ActionResult<Item> GetById(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public ActionResult<Item> Create([FromBody] Item item)
    {
        // Basic validation (mitigation for injection-style issues at the application layer)
        if (string.IsNullOrWhiteSpace(item.Name) || item.Name.Length > 100)
            return BadRequest("Name is required and must be <= 100 characters.");

        if (item.Quantity < 0)
            return BadRequest("Quantity cannot be negative.");

        item.Id = _items.Count == 0 ? 1 : _items.Max(i => i.Id) + 1;
        _items.Add(item);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }
}

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
