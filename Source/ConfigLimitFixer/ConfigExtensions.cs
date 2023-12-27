using System.Reflection;
using IPA.Config;

public static class ConfigExtensions
{
    private static FieldInfo StoreField { get; } = typeof(Config)
        .GetField("Store", BindingFlags.Instance | BindingFlags.NonPublic);

    public static IConfigStore GetInternalStore(this Config config)
    {
        return StoreField.GetValue(config) as IConfigStore;
    }
}
