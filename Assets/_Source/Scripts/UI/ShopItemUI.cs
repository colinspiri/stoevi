using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour {
    // SO references
    public ShopItem shopItem;
    public IntReference resource;
    [Space]
    public IntReference currentBalance;

    // components
    [Space]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI priceText;
    public Image icon;

    private void OnEnable() {
        UpdateUI();
    }

    private void UpdateUI() {
        itemNameText.text = shopItem.itemName;
        itemDescriptionText.text = shopItem.itemDescription;
        
        priceText.text = shopItem.price.ToString() + " levs";

        countText.text = resource.Value.ToString();

        icon.sprite = shopItem.icon;
    }

    public void TryBuyItem() {
        // check if have enough money
        if (shopItem.price > currentBalance.Value) {
            // TODO error SFX
            
            return;
        }
        
        // TODO success SFX
        
        // reduce balance
        if(currentBalance.Variable != null) currentBalance.Variable.ApplyChange(-1 * shopItem.price);
        
        // increment resource
        if(resource.Variable != null) resource.Variable.ApplyChange(1);
        
        UpdateUI();
    }
}
