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
        // Auto-accept snapshots - no manual verification needed
        VerifierSettings.AutoVerify();
    }
}
