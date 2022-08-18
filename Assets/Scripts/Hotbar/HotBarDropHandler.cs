using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HotBarDropHandler : MonoBehaviour, IDropHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private Hotbar hotbar;
    public Vector3 orgPosition;
    private void Awake()
    {
        hotbar = FindObjectOfType<Hotbar>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        HotSlot hotSlotFrom = eventData.pointerDrag.GetComponentInParent<HotSlot>();
        HotSlot hotSlotTo = GetComponentInParent<HotSlot>();

        if (hotSlotTo == null)
            print("hs_to is null");
        if (hotSlotFrom == null)
            print("hs_from is null");


        if (hotSlotTo != null && hotSlotFrom != null)
        {
            if (hotSlotFrom.isOccupied == false)
                return;

            if (hotSlotTo.isOccupied)
            {
                hotbar.SwapHotSlot(hotSlotFrom, hotSlotTo);
                return;
            }
            else
            {
                Item itemToInsert = hotSlotFrom.hotItem;
                hotSlotTo.InsertItem(itemToInsert);
                hotSlotFrom.ClearHotSlot();
                return;
            }
        }
        
        Slot slotFrom = eventData.pointerDrag.GetComponentInParent<Slot>();
        if (slotFrom != null)
        {
            if (slotFrom.slotItem != null)
            {
                if (slotFrom.slotItem.canEquip == false)
                    return;

                if (hotSlotTo.hotItem == null)
                {
                    hotSlotTo.InsertItem(slotFrom.slotItem);
                    slotFrom.ClearSlot();
                }
                else
                {
                    hotSlotTo.InsertItem(slotFrom.slotItem, true, slotFrom);
                }
            }
        }
    }

    private void ItemToInventorySlot(Slot slot, Item itemToInsertInSlot)
    {
        slot.AddItem(itemToInsertInSlot);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        hotbar.transform.SetAsLastSibling();
        orgPosition = transform.position;
        transform.parent.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = orgPosition;
        canvasGroup.blocksRaycasts = true;
    }
}
