using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace TeoTests;

[UsedImplicitly]
public class VerifyInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.InitializePlugins();
        VerifierSettings.UseStrictJson();
        VerifierSettings.ScrubInlineGuids();
        VerifierSettings.ScrubMembers("traceId");
        VerifierSettings.ScrubMembers("traceparent");
    }
}