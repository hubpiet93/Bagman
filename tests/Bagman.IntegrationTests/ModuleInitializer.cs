using System.Runtime.CompilerServices;
using Argon;
using VerifyTests;

namespace Bagman.IntegrationTests;

/// <summary>
/// Module initializer for integration tests configuration.
/// Sets up global Verify snapshot testing settings.
/// </summary>
public static class ModuleInitializer
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void Init()
    {
        VerifyHttp.Initialize();

        VerifierSettings.InitializePlugins();
        VerifierSettings.AutoVerify();
        VerifierSettings.AddExtraSettings(settings =>
        {
            settings.Formatting = Formatting.Indented;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
        });

    }
}
