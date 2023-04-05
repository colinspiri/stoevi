using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "ShopItem")]
public class ShopItem : ScriptableObject {
    public string itemName;
    public int price;
}
