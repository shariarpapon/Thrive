using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public static bool AllowSelection = true;

    public Camera mainCam;

    [SerializeField] private float detectionRadius = 100f;

    private Ray hoverRay;
    private RaycastHit hit;

    private InventoryManager inventory;

    private void Awake()
    {
        inventory = FindObjectOfType<InventoryManager>();
    }

    private void Update()
    {
       CheckToPickUp();
    }

    private void CheckToPickUp()
    {
        if (AllowSelection)
        {
            hoverRay = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(hoverRay, out hit, detectionRadius))
            {
                ItemIdentity itemID = hit.transform.GetComponent<ItemIdentity>();
                if (itemID != null)
                {
                    if (itemID.item.allowRawInteraction == false)
                        return;

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (inventory.AddToInventory(itemID.item))
                            Destroy(hit.transform.gameObject);
                    }
                }
            }
        }
    }
}
