using Microsoft.AspNetCore.Mvc.ApplicationModels;
using SlotGame.Api.Controllers;

namespace SlotGame.Api.Infrastructure;

/// <summary>
/// Removes <see cref="TestController"/> from the application model when not running
/// in the Development environment, so its routes are never registered and it never
/// appears in Swagger.
/// </summary>
public sealed class DevOnlyControllerConvention(bool isDevelopment) : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        if (isDevelopment)
        {
            return;
        }

        var testController = application.Controllers
            .FirstOrDefault(c => c.ControllerType == typeof(TestController));

        if (testController is not null)
        {
            application.Controllers.Remove(testController);
        }
    }
}
