using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using LLBML.Utils;
using LLGUI;
using TMPro;
using UnityEngine;

namespace CharacterStockIcons;

internal class IconPreviewWindow : MonoBehaviour
{
    private static readonly Vector2 POSITION = new Vector2(540f, 100f);
    private const float FONT_SIZE = 12f;

    internal static void Create(Transform tfParent)
    {
        RectTransform tf = LLControl.CreatePanel(tfParent, "iconPreviewUI", POSITION.x, POSITION.y);
        IconPreviewWindow window = tf.gameObject.AddComponent<IconPreviewWindow>();
        window.rectTransform = tf;
        window.Init();
    }

    private RectTransform rectTransform;
    private RectTransform tfContainer;
    private TextMeshProUGUI lbSelected;
    private TextMeshProUGUI lbPack;
    private Image imgBg;
    private List<IconPreview> iconPreviews;

    private void Init()
    {
        tfContainer = LLControl.CreatePanel(rectTransform, "container", 0f, 0f);
        imgBg = LLControl.CreateImage(tfContainer, Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f)));
        imgBg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160f);
        imgBg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 340f);
        imgBg.rectTransform.ForceUpdateRectTransforms();
        imgBg.rectTransform.pivot = new Vector2(0.55f, 1f);
        imgBg.rectTransform.localPosition = new Vector2(0f, 10f);
        imgBg.color = Color.black;
        
        RectTransform text = LLControl.CreatePanel(tfContainer, "lbSelected", -100f, 0f);
        lbSelected = text.gameObject.AddComponent<TextMeshProUGUI>();
        lbSelected.fontSize = FONT_SIZE;
        lbSelected.enableWordWrapping = false;
        lbSelected.alignment = TextAlignmentOptions.Right;
        lbSelected.color = Color.white;
        lbSelected.SetText("Stock icons:");
        
        RectTransform text2 = LLControl.CreatePanel(tfContainer, "lbPack", 106f, 0f);
        lbPack = text2.gameObject.AddComponent<TextMeshProUGUI>();
        lbPack.fontSize = FONT_SIZE;
        lbPack.enableWordWrapping = false;
        lbPack.alignment = TextAlignmentOptions.Left;
        lbPack.color = Color.red;
        lbPack.SetText("NULL");

        List<string> characterNames = IconPack.CharacterFromName.Keys.Where(n => n != "Empty").OrderBy(n => n).ToList();
        characterNames.Add("Empty");

        iconPreviews = new List<IconPreview>();
        for (int i = 0; i < characterNames.Count; i++)
        {
            iconPreviews.Add(new IconPreview(tfContainer, characterNames[i], 0f, -1 * (i+1) * StockIconContainer.StockIcon.ICON_SIZE));
        }
        
        tfContainer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!ModDependenciesUtils.InModOptions())
        {
            tfContainer.gameObject.SetActive(false);
            return;
        }
        
        tfContainer.gameObject.SetActive(true);
        lbPack.color = Plugin.Instance.selectedIconPack == null ? Color.red : Color.white;
        lbPack.SetText(Plugin.Instance.selectedIconPack == null ? "NULL" : Plugin.Instance.selectedIconPack.id);
        iconPreviews.ForEach(preview => preview.UpdatePack());
    }

    private class IconPreview
    {
        private RectTransform rectTransform;
        private TextMeshProUGUI lbCharacter;
        private string characterName;
        private StockIconContainer.StockIcon icon;

        internal IconPreview(Transform tfParent, string characterName, float x, float y)
        {
            rectTransform = LLControl.CreatePanel(tfParent, $"stockPreview_{characterName}", x, y);
            
            this.characterName = characterName;

            RectTransform text = LLControl.CreatePanel(rectTransform, "lbCharacter", -100f, 0f);
            lbCharacter = text.gameObject.AddComponent<TextMeshProUGUI>();
            lbCharacter.fontSize = FONT_SIZE;
            lbCharacter.color = Color.white;
            lbCharacter.alignment = TextAlignmentOptions.Right;
            lbCharacter.SetText(characterName);
            
            icon = StockIconContainer.StockIcon.CreateStockIcon(rectTransform, $"stockIcon_{characterName}", 0);
            icon.rectTransform.localPosition = new Vector2(16f, 0f);
            icon.SetDisplay(characterName != "Empty");
        }

        internal void UpdatePack()
        {
            if (Plugin.Instance.selectedIconPack == null)
            {
                icon.gameObject.SetActive(false);
                return;
            }
            
            icon.gameObject.SetActive(true);
            icon.SetCharacter(IconPack.CharacterFromName[characterName]);
        }
    }
}