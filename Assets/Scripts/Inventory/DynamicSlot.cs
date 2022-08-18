using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Slot mySlot;
    private CanvasGroup canvasGroup;
    public bool beingDragged;
    //public Vector3 originalPosition = Vector3.zero;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        mySlot = transform.parent.GetComponent<Slot>();
    }

    private void OnDisable()
    {
        transform.localPosition = Vector3.zero;
        beingDragged = false;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        FindObjectOfType<InventoryButtonEvents>().transform.SetAsLastSibling();
        transform.parent.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
        beingDragged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(beingDragged)
            transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragEndEvent();
    }

    public void DragEndEvent()
    {
        transform.localPosition = Vector3.zero;
        Slot _slot = GetComponentInParent<Slot>();
        if (_slot.slotItem == null)
        {
            _slot.ClearSlot();
        }
        canvasGroup.blocksRaycasts = true;
        beingDragged = false;
    }
}
