using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [HideInInspector]
    public Item equipedItem;
    public Transform equipmentHolder;

    private Hotbar hotbar;
    internal Animator activeAnimator;

    private void Awake()
    {
        hotbar = FindObjectOfType<Hotbar>();
    }

    private void Start()
    {
        UpdateActiveEquipment();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
            UpdateActiveEquipment();
    }

    public void UpdateActiveEquipment()
    {
        if (hotbar.SelectedItem())
        {
            print("Checking to Equip, Hotbar has selected item, going to destroy current item");
            equipedItem = hotbar.SelectedItem();
            DestroyCurrentItem();
            EquipItem();
        }
        else
        {
            print("Hotbar does not have selected item, going to destroy curren titem and set equipedItem to null");
            equipedItem = null;
            activeAnimator = null;
            DestroyCurrentItem();
            print("NO ITEM IN HOTBAR");
            return;
        }
    }

    private void EquipItem()
    {
        if (equipedItem != null)
        {
            if (equipedItem.itemPrefab == null)
                return;
            print("equipedItem is not null");

            GameObject res = Resources.Load(equipedItem.name) as GameObject;
            if (res == null)
                return;
            GameObject instance = Instantiate(res, new Vector3(0.0f, 100.0f, 0.0f), Quaternion.identity, equipmentHolder) as GameObject;
            activeAnimator = instance.GetComponent<Animator>();
            if(activeAnimator != null)
                activeAnimator.enabled = true;
            ItemIdentity itemID = instance.GetComponent<ItemIdentity>();
            if (itemID != null)
                itemID.Destroy();
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
        }
        else
            print("equipedItem is null");
    }

    private void DestroyCurrentItem()
    {
        if (equipmentHolder.childCount > 0)
            Destroy(equipmentHolder.GetChild(0).gameObject);
    }
}
