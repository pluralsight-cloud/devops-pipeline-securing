using Microsoft.AspNetCore.Mvc;

namespace SampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    // In-memory store for the sample
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

    // -------------------------------------------------------------------------
    // DEMO ONLY: Uncomment the method below to make the CI check fail.
    // This demonstrates the threat described in threat-model.md.
    // -------------------------------------------------------------------------

    //[HttpGet("search")]
    //public ActionResult Search(string query)
    //{
    //    // Unsafe: concatenating user input into a query-like string
    //    var sql = "SELECT * FROM Items WHERE Name LIKE '%" + query + "%'";
    //    return Ok(new { warning = $"This would have executed this query: '{ sql}'", sql });
    //}

}

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
