using HarmonyLib;
using LLBML.Players;
using LLBML.Settings;

namespace CharacterStockIcons;

internal static class HarmonyPatches
{
    internal static void PatchAll()
    {
        Harmony harmony = new Harmony(Plugin.GUID);
        
        harmony.PatchAll(typeof(StockDisplayPatch));
        Plugin.LogGlobal.LogInfo("Stock display patch applied");
    }

    private static class StockDisplayPatch
    {
        [HarmonyPatch(typeof(GameHudPlayerInfo), nameof(GameHudPlayerInfo.Awake))]
        [HarmonyPostfix]
        private static void Awake_Postfix(GameHudPlayerInfo __instance)
        {
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
            StockIconContainer stockIconContainer = __instance.transform.GetComponentInChildren<StockIconContainer>();
            stockIconContainer.ShowStocks(stocks);
        }
    }
}