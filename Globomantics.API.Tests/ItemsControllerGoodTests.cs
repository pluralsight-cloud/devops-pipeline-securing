using Xunit;
using SampleApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Globomantics.API.Tests;

public class ItemsControllerGoodTests : ItemsControllerTestBase
{
    [Fact]
    public void Get_returns_the_seeded_items()
    {
        var controller = new ItemsController();

        var result = controller.Get().Result;

        var response = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsAssignableFrom<IEnumerable<Item>>(response.Value);
        Assert.Collection(
            items,
            item =>
            {
                Assert.Equal(1, item.Id);
                Assert.Equal("Widget", item.Name);
                Assert.Equal(10, item.Quantity);
            },
            item =>
            {
                Assert.Equal(2, item.Id);
                Assert.Equal("Gadget", item.Name);
                Assert.Equal(5, item.Quantity);
            });
    }

    [Fact]
    public void GetById_returns_the_matching_item()
    {
        var controller = new ItemsController();

        var result = controller.GetById(2);

        var response = Assert.IsType<OkObjectResult>(result.Result);
        var item = Assert.IsType<Item>(response.Value);
        Assert.Equal(2, item.Id);
        Assert.Equal("Gadget", item.Name);
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void GetById_returns_not_found_for_an_unknown_id()
    {
        var controller = new ItemsController();

        var result = controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_rejects_a_missing_name(string name)
    {
        var controller = new ItemsController();

        var result = controller.Create(new Item { Name = name, Quantity = 1 });

        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Name is required and must be <= 100 characters.", response.Value);
    }

    [Fact]
    public void Create_rejects_a_negative_quantity()
    {
        var controller = new ItemsController();

        var result = controller.Create(new Item { Name = "Invalid", Quantity = -1 });

        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Quantity cannot be negative.", response.Value);
    }

    [Fact]
    public void Create_assigns_an_id_and_returns_created_item()
    {
        var controller = new ItemsController();
        var item = new Item { Name = "Created", Quantity = 3 };

        var result = controller.Create(item);

        var response = Assert.IsType<CreatedAtActionResult>(result.Result);
        var created = Assert.IsType<Item>(response.Value);
        Assert.Equal("GetById", response.ActionName);
        Assert.Equal(created.Id, response.RouteValues!["id"]);
        Assert.Equal("Created", created.Name);
        Assert.Equal(3, created.Quantity);
        Assert.True(created.Id > 2);
    }
}
