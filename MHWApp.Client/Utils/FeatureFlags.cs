namespace MHWApp.Client.Utils
{
    public static class FeatureFlags
    {
        #if ENABLE_REGISTER
            public const bool IsRegisterEnabled = true;
        #else
            public const bool IsRegisterEnabled = false;
        #endif
    }
}