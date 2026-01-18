using System.Text;
using System.Text.RegularExpressions;

namespace Bagman.IntegrationTests;

public static class VerifierSettingsExtensions
{
    public static void ScrubPropInJsonObjectWhenString(this StringBuilder sb, string propertyName)
    {
        var pattern = $"""{propertyName}"\s*:\s*"[^"]*""";
        var replacement = $"""{propertyName}":"***SCRUBBED***""";
        var result = Regex. Replace(sb.ToString(), pattern, replacement);
        sb.Clear();
        sb.Append(result);
    }
    
    public static void ScrubBearerToken(this StringBuilder sb)
    {
        var text = sb.ToString();
        var pattern = "Bearer eyJ[^\\s]+";
        var replacement = "Bearer ***SCRUBBED***";
        var result = Regex.Replace(text, pattern, replacement);
        sb.Clear();
        sb.Append(result);
    }
}
