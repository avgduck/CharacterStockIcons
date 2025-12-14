using System.Collections.Generic;
using LLGUI;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterStockIcons;

internal class StockIconContainer : MonoBehaviour
{
    private List<StockIcon> stockIcons;
    
    internal static StockIconContainer CreateStockIconContainer(Transform tfParent, string name, int maxStocks)
    {
        RectTransform tf = LLControl.CreatePanel(tfParent, name, -54, -100);
        
        StockIconContainer stockIconContainer = tf.gameObject.AddComponent<StockIconContainer>();
        stockIconContainer.stockIcons = new List<StockIcon>();
        for (int i = 0; i < 10; i++)
        {
            StockIcon icon = StockIcon.CreateStockIcon(tf, $"stockIcon_{i}", i);
            stockIconContainer.stockIcons.Add(icon);

            icon.rectTransform.localPosition = new Vector2(StockIcon.ICON_SIZE * i, 0);
            if (i > maxStocks - 1) icon.gameObject.SetActive(false);
        }

        return stockIconContainer;
    }

    internal void SetCharacter(Character character)
    {
        stockIcons.ForEach(icon => icon.SetCharacter(character));
    }

    internal void ShowStocks(int stocks)
    {
        if (stocks <= 0) stockIcons.ForEach(icon => icon.gameObject.SetActive(false));
        else stockIcons.ForEach(icon => icon.SetDisplay(icon.index < stocks));
    }

    internal class StockIcon : MonoBehaviour
    {
        internal const float ICON_SIZE = 24;
        
        internal RectTransform rectTransform;
        internal Image imgChar;
        internal Image imgEmpty;
        internal int index;
        
        internal static StockIcon CreateStockIcon(Transform tfParent, string name, int index)
        {
            RectTransform container = LLControl.CreatePanel(tfParent, name);
        
            StockIcon icon = container.gameObject.AddComponent<StockIcon>();
            icon.rectTransform = container;
            icon.index = index;
            
            icon.imgChar = LLControl.CreateImage(container, Plugin.Instance.defaultIcon);
            icon.imgChar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ICON_SIZE);
            icon.imgChar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ICON_SIZE);
            icon.imgChar.rectTransform.localPosition = new Vector2(0f, 0f);
            
            icon.imgEmpty = LLControl.CreateImage(container, Plugin.Instance.defaultIcon);
            icon.imgEmpty.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ICON_SIZE);
            icon.imgEmpty.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ICON_SIZE);
            icon.imgEmpty.rectTransform.localPosition = new Vector2(0f, 0f);

            return icon;
        }

        internal void SetCharacter(Character character)
        {
            imgChar.sprite = Plugin.Instance.selectedIconPack.icons[character];
            imgEmpty.sprite = Plugin.Instance.selectedIconPack.icons[Character.NONE];
        }
        
        internal void SetDisplay(bool active)
        {
            imgChar.gameObject.SetActive(active);
            imgEmpty.gameObject.SetActive(!active);
        }
    }
}