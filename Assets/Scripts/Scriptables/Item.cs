using UnityEngine;

public enum ItemType
{
    Null,
    Sword,
    Spear,
    Consumable,
    Placeable,
    Buildable,
    Pickaxe,
    Axe
}

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptables/Items/Item")]
public class Item : ScriptableObject
{
    public new string name;
    [Range(1, 20)]
    public int spaceCost;
    [Tooltip("An item prefab is not required but needs one if this item is meant to be dropped as an gameobject in-game")]
    public GameObject itemPrefab;
    public Sprite icon;
    [TextArea]
    public string description;

    [Space]
    [Header("Equipment Stats :")]
    public ItemType itemType;
    public bool canEquip;
    public bool isInteractable;
    public bool allowRawInteraction;
    public int durability;
    public int damage;
    public int dph;

    [Space]
    [Header("SFX :")]
    public bool build_SFX;
    public AudioSet buildSFX;

    public virtual void Use(Slot slot)
    {
        switch (itemType)
        {
            default:
                Debug.Log("Trying to use " + name);
                break;

            case ItemType.Placeable:
                Place(slot);
                break;

            case ItemType.Consumable:
                Consume(slot);
                break;

            case ItemType.Buildable:
                Build(slot);
                break;
        }
    }

    public void Consume(Slot slot)
    {
        ConsumableManager.instance.Consume(slot);
    }

    public void Place(Slot slot)
    {
        BuildingManager.instance.InitiatePreview(itemPrefab, slot);
    }
    public void Build(Slot slot)
    {
        Debug.Log("trying to build " + name);
    } 
}
