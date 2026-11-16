using SampleApi.Controllers;

namespace Globomantics.API.Tests;

public abstract class ItemsControllerTestBase
{
    protected ItemsControllerTestBase()
    {
        ItemsController.Reset();
    }
}
