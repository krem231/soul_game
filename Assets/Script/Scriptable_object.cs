using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Scriptable_object : ScriptableObject
{
    [Header("Item Basic Info")]
    public string item_name = "New Item";
    public Sprite itemIcon;  

    [TextArea(3, 5)]
    public string description = "Item description here";

    [Header("Item Properties")]
    public ItemType itemType;
    public bool IsUseableAnywhere = false;
    public int maxStack = 99;

    [Header("Item Stats (Optional)")]
    public int healAmount = 0;
    public int damage = 0;
    public int defense = 0;
}


public enum ItemType
{
    Consumable,   
    Weapon,       
    Armor,        
    Material,     
    Quest,        
    Gem,           
    UpgradeItem    
}