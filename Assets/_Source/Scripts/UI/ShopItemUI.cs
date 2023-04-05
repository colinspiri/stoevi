using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopItemUI : MonoBehaviour {
    // SO references
    public ShopItem shopItem;
    public IntReference resource;
    [Space]
    public IntReference currentBalance;

    // components
    [Space]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI priceText;

    private void OnEnable() {
        UpdateUI();
    }

    private void UpdateUI() {
        itemNameText.text = shopItem.itemName;
        priceText.text = shopItem.price.ToString() + " Lv";

        countText.text = resource.Value.ToString();
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
