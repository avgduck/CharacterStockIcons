using BepInEx.Configuration;

namespace CharacterStockIcons;

internal static class Configs
{
    internal static ConfigEntry<string> SelectedIconPackId { get; private set; }

    internal static void BindConfigs()
    {
        ConfigFile config = Plugin.Instance.Config;

        SelectedIconPackId = config.Bind<string>("Settings", "SelectedIconPackId", "stadium", "Selected icon pack (default or custom) specified by the id, which is based on the name of the icon pack subdirectory");
    }
}