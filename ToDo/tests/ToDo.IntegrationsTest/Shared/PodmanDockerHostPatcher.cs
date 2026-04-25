namespace ToDo.IntegrationsTest.Shared;

public static class PodmanDockerHostPatcher
{
    public static bool EnsurePatched() => Patch();

    private static bool Patch()
    {
        const string badPrefix = "npipe:////";
        const string goodPrefix = "npipe://";

        try
        {
            var dockerHost = Environment.GetEnvironmentVariable("DOCKER_HOST") ?? string.Empty;
            if (!dockerHost.StartsWith(badPrefix, StringComparison.OrdinalIgnoreCase)) return true;

            var normalized = goodPrefix + dockerHost[badPrefix.Length..];
            Environment.SetEnvironmentVariable("DOCKER_HOST", normalized);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}