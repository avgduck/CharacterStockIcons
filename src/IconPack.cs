using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CharacterStockIcons;

internal class IconPack
{
    internal static readonly Dictionary<string, Character> CharacterFromName = new Dictionary<string, Character>()
    {
        {"Candyman", Character.CANDY},
        {"Dice", Character.PONG},
        {"Doombox", Character.BOSS},
        {"DustAshes", Character.BAG},
        {"Grid", Character.ELECTRO},
        {"Jet", Character.SKATE},
        {"Latch", Character.CROC},
        {"Nitro", Character.COP},
        {"Raptor", Character.KID},
        {"Sonata", Character.BOOM},
        {"Switch", Character.ROBOT},
        {"Toxic", Character.GRAF},
        
        //{"Random", Character.RANDOM},
        {"Empty", Character.NONE}
    };

    internal string id;
    internal Dictionary<Character, Sprite> icons;
    internal bool isValid;

    internal IconPack(DirectoryInfo directory)
    {
        id = directory.Name;
        icons = new Dictionary<Character, Sprite>();
        isValid = false;
        
        Plugin.LogGlobal.LogInfo($"Attempting to load icon pack: '{id}'");
        
        foreach (FileInfo file in directory.GetFiles().OrderBy(f => f.Name))
        {
            //Plugin.LogGlobal.LogInfo($"Checking icon file: '{file.Name}'");
            
            string characterName;
            if (file.Name.Contains(".png")) characterName = file.Name.Replace(".png", "");
            else if (file.Name.Contains(".jpg")) characterName = file.Name.Replace(".jpg", "");
            else
            {
                Plugin.LogGlobal.LogWarning($"Icon file '{file.Name}' invalid: extension must be .png or .jpg");
                continue;
            }
            
            
            if (!CharacterFromName.ContainsKey(characterName))
            {
                Plugin.LogGlobal.LogWarning($"Icon file '{file.Name}' invalid: '{characterName}' is not a valid character");
                continue;
            }

            Texture2D tex = LoadImageFile(file);
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            icons.Add(CharacterFromName[characterName], sprite);
            //Plugin.LogGlobal.LogInfo($"Icon file '{file.Name}' loaded");
        }

        bool isMissingCharacters = false;
        foreach (KeyValuePair<string, Character> entry in CharacterFromName)
        {
            if (!icons.ContainsKey(entry.Value))
            {
                Plugin.LogGlobal.LogWarning($"Icon pack '{id}' is missing an icon for character '{entry.Key}'");
                isMissingCharacters = true;
            }
        }
        if (isMissingCharacters) return;

        isValid = true;
    }
    
    private static void CopyStream(Stream input, Stream output)
    {
        byte[] buffer = new byte[8 * 1024];
        int len;
        while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, len);
        }
    }

    private static Texture2D LoadImageFile(FileInfo file)
    {
        //Plugin.LogGlobal.LogInfo($"Loading image file: {file.FullName}");
        using FileStream fileStream = file.OpenRead();
        using MemoryStream memoryStream = new MemoryStream();
        
        CopyStream(fileStream, memoryStream);
        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(memoryStream.ToArray());
        return tex;
    }
}