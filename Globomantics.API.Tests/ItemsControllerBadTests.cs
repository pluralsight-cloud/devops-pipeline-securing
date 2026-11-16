using Xunit;
using SampleApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Globomantics.API.Tests;

// These tests are intentionally weak examples: they execute code, but their assertions
// are too broad to protect behavior from common mutations.
public class ItemsControllerBadTests : ItemsControllerTestBase
{
    [Fact]
    [Trait("TestQuality", "Bad")]
    public void Get_returns_a_result()
    {
        var controller = new ItemsController();

        var result = controller.Get();

        Assert.NotNull(result);
    }

    [Fact]
    [Trait("TestQuality", "Bad")]
    public void GetById_unknown_id_returns_something()
    {
        var controller = new ItemsController();

        var result = controller.GetById(-1);

        Assert.NotNull(result);
    }

    [Fact]
    [Trait("TestQuality", "Bad")]
    public void Create_invalid_item_returns_an_action_result()
    {
        var controller = new ItemsController();

        var result = controller.Create(new Item { Name = "", Quantity = -1 });

        Assert.NotNull(result);
    }

    [Fact]
    [Trait("TestQuality", "Bad")]
    public void Search_returns_ok_without_checking_the_query_behavior()
    {
        var controller = new ItemsController();

        var result = controller.Search("' OR 1 = 1 --");

        Assert.IsType<OkResult>(result);
    }
}
