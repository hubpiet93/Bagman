using System.Runtime.CompilerServices;
using Argon;

namespace Bagman.IntegrationTests;

/// <summary>
///     Module initializer for integration tests configuration.
///     Sets up global Verify snapshot testing settings.
/// </summary>
public static class ModuleInitializer
{
    [ModuleInitializer]
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
        VerifierSettings.ScrubMember("Authorization");
        VerifierSettings.ScrubMember("accessToken");
        VerifierSettings.ScrubMember("RefreshToken");
        VerifierSettings.ScrubMember("refreshToken");
        VerifierSettings.ScrubMember("Login");
        VerifierSettings.ScrubMember("UserLogin");
        VerifierSettings.ScrubMember("login");
        VerifierSettings.ScrubMember("userLogin");
        VerifierSettings.ScrubMember("Email");
        VerifierSettings.ScrubMember("email");
        VerifierSettings.ScrubMember("traceId");
        VerifierSettings.ScrubInlineDateTimes("yyyy-MM-ddTHH:mm:ss.FFFFFFZ");
    }
}
