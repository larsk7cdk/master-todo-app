namespace ToDo.IntegrationsTest.Shared;

public static class PodmanDockerHostPatcher
{
    private static readonly bool Patched = Patch();

    public static bool EnsurePatched() => Patched;

    private static bool Patch()
    {
        const string badPrefix = "npipe:////";
        const string goodPrefix = "npipe://";

        var dockerHost = Environment.GetEnvironmentVariable("DOCKER_HOST") ?? string.Empty;
        if (dockerHost.StartsWith(badPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var normalized = goodPrefix + dockerHost[badPrefix.Length..];
            Environment.SetEnvironmentVariable("DOCKER_HOST", normalized);
        }

        return true;
    }
}