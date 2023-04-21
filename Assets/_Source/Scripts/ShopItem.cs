using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "ShopItem")]
public class ShopItem : ScriptableObject {
    public string itemName;
    public string itemDescription;
    public int price;
    public Sprite icon;
}
