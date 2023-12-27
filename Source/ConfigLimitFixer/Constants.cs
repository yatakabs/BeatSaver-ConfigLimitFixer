namespace ConfigLimitFixer;

public static class Constants
{
    public const string ConfigRuntimeTypeFullName = "IPA.Config.ConfigRuntime";
    public const string SaveThreadFieldName = "saveThread";

    public const string ConfigsFieldName = "configs";
    public const string SaveMethodName = "Save";
    public const string ConfigsChangedWatcherFieldName = "configsChangedWatcher";
    public const string ReplacedSaveThreadName = nameof(ConfigLimitFixer) + "::ReplacedSaveThread";
}
