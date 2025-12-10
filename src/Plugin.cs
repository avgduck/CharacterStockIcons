using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using LLBML.Utils;

namespace CharacterStockIcons;

[BepInPlugin(GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(DEPENDENCY_LLBML, BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency(DEPENDENCY_MODMENU, BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    public const string GUID = "avgduck.plugins.llb.characterstockicons";
    internal const string DEPENDENCY_LLBML = "fr.glomzubuk.plugins.llb.llbml";
    internal const string DEPENDENCY_MODMENU = "no.mrgentle.plugins.llb.modmenu";
    
    public static Plugin Instance { get; private set; }
    internal static ManualLogSource LogGlobal { get; private set; }

    private DirectoryInfo packDirectoryDefault;
    private DirectoryInfo packDirectoryCustom;

    private Dictionary<string, IconPack> iconPacksDefault;
    private Dictionary<string, IconPack> iconPacksCustom;

    internal IconPack selectedIconPack;

    private void Awake()
    {
        Instance = this;
        LogGlobal = this.Logger;
        
        HarmonyPatches.PatchAll();

        string pathDefault = Path.Combine(Path.GetDirectoryName(Info.Location), "packs-default");
        if (!Directory.Exists(pathDefault)) Directory.CreateDirectory(pathDefault);
        packDirectoryDefault = new DirectoryInfo(pathDefault);

        DirectoryInfo moddingFolder = LLBML.Utils.ModdingFolder.GetModSubFolder(Info);
        string pathCustom = Path.Combine(moddingFolder.FullName, "packs-custom");
        if (!Directory.Exists(pathCustom)) Directory.CreateDirectory(pathCustom);
        packDirectoryCustom = new DirectoryInfo(pathCustom);
        
        LoadAllIconPacks();
        if (iconPacksDefault.Count > 0) selectedIconPack = iconPacksDefault.First().Value;
        
        Configs.BindConfigs();
        Config.SettingChanged += (sender, args) => OnConfigChanged();
        OnConfigChanged();
        ModDependenciesUtils.RegisterToModMenu(Info, GetModMenuText());
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

    private List<string> GetModMenuText()
    {
        List<string> text = new List<string>();

        text.Add("Choose an icon pack from those currently loaded (shown below). Default icon packs are included with the mod download, and custom icon packs can be added to your Modding Folder.");
        
        text.Add("");
        text.Add("");
        
        text.Add("<b>Default Icon Packs:</b>");
        if (iconPacksDefault.Count == 0)
        {
            text.Add("none");
        }
        else
        {
            iconPacksDefault.ToList().ForEach(entry => text.Add($"- {entry.Key}"));
        }
        
        text.Add("");
        
        text.Add("<b>Custom Icon Packs:</b>");
        if (iconPacksCustom.Count == 0)
        {
            text.Add("none");
        }
        else
        {
            iconPacksCustom.ToList().ForEach(entry => text.Add($"- {entry.Key}"));
        }
        
        return text;
    }

    private void OnConfigChanged()
    {
        string id = Configs.SelectedIconPackId.Value;
        if (iconPacksDefault.ContainsKey(id))
        {
            LogGlobal.LogInfo($"Selected default icon pack '{id}'");
            selectedIconPack = iconPacksDefault[id];
        }
        else if (iconPacksCustom.ContainsKey(id))
        {
            LogGlobal.LogInfo($"Selected custom icon pack '{id}'");
            selectedIconPack = iconPacksCustom[id];
        }
        else
        {
            //LogGlobal.LogError($"Icon pack '{id}' does not exist!");
            selectedIconPack = null;
        }
    }
}