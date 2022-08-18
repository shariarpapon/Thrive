using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicSlotHolder : MonoBehaviour, IDropHandler
{
    private Slot mySlot;

    private void Awake()
    {
        mySlot = GetComponent<Slot>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Slot dragSlot = eventData.pointerDrag.GetComponentInParent<Slot>();
        if (dragSlot != null && dragSlot != GetComponent<Slot>())
        {
            dragSlot.CheckToSwap(mySlot, Vector3.zero, eventData.pointerDrag.GetComponent<CanvasGroup>());
            print("Swapping ISlot to ISlot");
            return;
        }
        HotSlot hotSlot = eventData.pointerDrag.GetComponentInParent<HotSlot>();
        if (hotSlot != null)
        {
            if (mySlot.slotItem == null)
            {
                mySlot.AddItem(hotSlot.hotItem);
                hotSlot.ClearHotSlot();
                InventoryManager.instance.UpdateSlotValues(mySlot);
            }
            else if (mySlot.slotItem != null)
            {
                if (mySlot.slotItem.canEquip == false)
                    return;

                mySlot.slotIcon.gameObject.SetActive(true);
                Item cItem = mySlot.slotItem;
                mySlot.slotItem = hotSlot.hotItem;
                mySlot.itemCount = 1;
                mySlot.slotIcon.sprite = hotSlot.hotItem.icon;
                mySlot.spaceLeft = 0;
                mySlot.stackable = false;

                hotSlot.hotItem = cItem;
                hotSlot.slotImage.sprite = cItem.icon;
            }
        }
    }

}
