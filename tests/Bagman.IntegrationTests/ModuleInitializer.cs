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
        
        VerifierSettings.ScrubInlineGuids();
        VerifierSettings.AddScrubber(sb => sb.ScrubPropInJsonObjectWhenString("refreshToken"));
        VerifierSettings.AddScrubber(sb => sb.ScrubPropInJsonObjectWhenString("accessToken"));
        VerifierSettings.AddScrubber(sb => sb.ScrubPropInJsonObjectWhenString("Login"));
        VerifierSettings.AddScrubber(sb => sb.ScrubPropInJsonObjectWhenString("login"));
        VerifierSettings.AddScrubber(sb => sb.ScrubPropInJsonObjectWhenString("Email"));
        VerifierSettings.AddScrubber(sb => sb.ScrubPropInJsonObjectWhenString("email"));
        VerifierSettings.AddScrubber(sb => sb.ScrubBearerToken());
        VerifierSettings.ScrubInlineDateTimes(format: "yyyy-MM-ddTHH:mm:ss.FFFFFFZ");
    }
        
}