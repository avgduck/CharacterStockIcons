using BepInEx;
using HarmonyLib;
using LLBML.Players;
using LLBML.Settings;
using LLBML.Utils;

namespace CharacterStockIcons;

internal static class HarmonyPatches
{
    internal static void PatchAll()
    {
        Harmony harmony = new Harmony(Plugin.GUID);
        
        harmony.PatchAll(typeof(StockDisplayPatch));
        Plugin.LogGlobal.LogInfo("Stock display patch applied");

        if (!ModDependenciesUtils.IsModLoaded(Plugin.DEPENDENCY_MODMENU))
        {
            Plugin.LogGlobal.LogWarning("ModMenu is not loaded. Skipping preview patch...");
            return;
        }
        
        harmony.PatchAll(typeof(IconPreviewPatch));
        Plugin.LogGlobal.LogInfo("Icon preview patch applied");
    }

    private static class StockDisplayPatch
    {
        [HarmonyPatch(typeof(GameHudPlayerInfo), nameof(GameHudPlayerInfo.Awake))]
        [HarmonyPostfix]
        private static void Awake_Postfix(GameHudPlayerInfo __instance)
        {
            if (Plugin.Instance.selectedIconPack == null)
            {
                Plugin.LogGlobal.LogError("No valid icon pack selected! Check that the id is set correctly in the config");
                return;
            }
            
            if (!__instance.showScore) return;
            if (GameSettings.current.UsePoints) return;
            
            __instance.imStocksBackdrop.gameObject.SetActive(false);
            __instance.imStocks.gameObject.SetActive(false);
            
            StockIconContainer stockIconContainer = StockIconContainer.CreateStockIconContainer(__instance.gameObject.transform, "stockIconContainer", GameSettings.current.stocks);
        }

        [HarmonyPatch(typeof(GameHudPlayerInfo), nameof(GameHudPlayerInfo.SetPlayer))]
        [HarmonyPostfix]
        private static void SetPlayer_Postfix(GameHudPlayerInfo __instance)
        {
            if (Plugin.Instance.selectedIconPack == null) return;
            if (!__instance.showScore) return;
            if (GameSettings.current.UsePoints) return;
            
            Player player = __instance.shownPlayer;
            
            StockIconContainer stockIconContainer = __instance.transform.GetComponentInChildren<StockIconContainer>();
            stockIconContainer.SetCharacter(player.Character);
        }

        [HarmonyPatch(typeof(GameHudPlayerInfo), nameof(GameHudPlayerInfo.ShowStocks))]
        [HarmonyPostfix]
        private static void ShowStocks_Postfix(GameHudPlayerInfo __instance, int stocks)
        {
            if (Plugin.Instance.selectedIconPack == null) return;
            
            StockIconContainer stockIconContainer = __instance.transform.GetComponentInChildren<StockIconContainer>();
            stockIconContainer.ShowStocks(stocks);
        }
    }

    internal static class IconPreviewPatch
    {
        internal static bool isInModMenu = false;
        
        [HarmonyPatch(typeof(ModMenu.ModMenu), "HandleModSubSettingsClick")]
        [HarmonyPrefix]
        private static void HandleModSubSettingsClick_Prefix(PluginInfo plugin)
        {
            isInModMenu = plugin.Metadata.GUID == Plugin.GUID;
        }

        [HarmonyPatch(typeof(ModMenu.ModMenu), "HandleQuitClick")]
        [HarmonyPrefix]
        private static void HandleQuitClick()
        {
            isInModMenu = false;
        }
        
        [HarmonyPatch(typeof(ScreenMenu), nameof(ScreenMenu.OnOpen))]
        [HarmonyPostfix]
        private static void OnOpen_Postfix(ScreenMenu __instance)
        {
            IconPreviewWindow.Create(__instance.gameObject.transform);
        }
    }
}