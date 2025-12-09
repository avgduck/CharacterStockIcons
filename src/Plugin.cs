using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;

namespace CharacterStockIcons;

[BepInPlugin(GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("fr.glomzubuk.plugins.llb.llbml", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("no.mrgentle.plugins.llb.modmenu", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    public const string GUID = "avgduck.plugins.llb.tourneyinfo";
    public static Plugin Instance { get; private set; }
    internal static ManualLogSource LogGlobal { get; private set; }

    private DirectoryInfo packDirectoryDefault;
    private DirectoryInfo packDirectoryCustom;

    internal Dictionary<string, IconPack> iconPacksDefault;
    internal Dictionary<string, IconPack> iconPacksCustom;

    internal IconPack selectedIconPack;

    private void Awake()
    {
        Instance = this;
        LogGlobal = this.Logger;

        string pathDefault = Path.Combine(Path.GetDirectoryName(Info.Location), "packs-default");
        if (!Directory.Exists(pathDefault)) Directory.CreateDirectory(pathDefault);
        packDirectoryDefault = new DirectoryInfo(pathDefault);

        DirectoryInfo moddingFolder = LLBML.Utils.ModdingFolder.GetModSubFolder(Info);
        string pathCustom = Path.Combine(moddingFolder.FullName, "packs-custom");
        if (!Directory.Exists(pathCustom)) Directory.CreateDirectory(pathCustom);
        packDirectoryCustom = new DirectoryInfo(pathCustom);
        
        LoadAllIconPacks();
        if (iconPacksDefault.Count > 0) selectedIconPack = iconPacksDefault.First().Value;
        
        HarmonyPatches.PatchAll();
    }

    private void LoadAllIconPacks()
    {
        iconPacksDefault = new Dictionary<string, IconPack>();
        foreach (DirectoryInfo packDir in packDirectoryDefault.GetDirectories().OrderBy(d => d.Name))
        {
            IconPack pack = new IconPack(packDir);

            if (iconPacksDefault.ContainsKey(pack.id))
            {
                LogGlobal.LogWarning($"Failed to load default icon pack '{pack.id}': pack with id '{pack.id}' already exists");
                continue;
            }
            if (!pack.isValid)
            {
                LogGlobal.LogWarning($"Failed to load default icon pack '{pack.id}': invalid pack format");
                continue;
            }
            
            LogGlobal.LogInfo($"Loaded default icon pack '{pack.id}'");
            iconPacksDefault.Add(pack.id, pack);
        }

        iconPacksCustom = new Dictionary<string, IconPack>();
        foreach (DirectoryInfo packDir in packDirectoryCustom.GetDirectories().OrderBy(d => d.Name))
        {
            IconPack pack = new IconPack(packDir);
            
            if (iconPacksDefault.ContainsKey(pack.id) || iconPacksCustom.ContainsKey(pack.id))
            {
                LogGlobal.LogWarning($"Failed to load custom icon pack '{pack.id}': pack with id '{pack.id}' already exists");
                continue;
            }
            if (!pack.isValid)
            {
                LogGlobal.LogWarning($"Failed to load custom icon pack '{pack.id}'");
                continue;
            }
            
            LogGlobal.LogInfo($"Loaded custom icon pack '{pack.id}'");
            iconPacksCustom.Add(pack.id, pack);
        }
    }
}