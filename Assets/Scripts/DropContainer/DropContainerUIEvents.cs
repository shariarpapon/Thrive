using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropContainerUIEvents : MonoBehaviour
{
    public GameObject mainUI;
    public GameObject contentUI;
    public GameObject itemButtonPrefab;

    private DropContainerManager manager;
    private InventoryManager inventory;

    internal bool containerIsOpen = false;

    private void Awake()
    {
        manager = FindObjectOfType<DropContainerManager>();
        inventory = FindObjectOfType<InventoryManager>();
    }

    public void Open()
    {
        if (mainUI.activeSelf == false && !inventory.InventoryIsOpen() && containerIsOpen == false)
        {
            Cursor.lockState = CursorLockMode.None;
            InventoryManager.AllowAccess = false;
            FPController.RestrictControls = true;
            FPMouseLook.RestrictRotation = true;
            PickUpItem.AllowSelection = false;
            mainUI.SetActive(true);
            containerIsOpen = true;
        }
    }

    public void Close()
    {
        Clear();
        manager.UpdateContainer(null);
        if(mainUI.activeSelf)
            mainUI.SetActive(false);

        PickUpItem.AllowSelection = true;
        InventoryManager.AllowAccess = true;
        FPController.RestrictControls = false;
        FPMouseLook.RestrictRotation = false;
        Cursor.lockState = CursorLockMode.Locked;
        containerIsOpen = false;
    }

    public void Clear()
    {
        if (contentUI.transform.childCount > 0)
        {
            for (int i = 0; i < contentUI.transform.childCount; i++)
            {
                Destroy(contentUI.transform.GetChild(i).gameObject);
            }
        }
    }

    public void AddItemButtons(Drop drop)
    {
        TextMeshProUGUI buttonText = Instantiate(itemButtonPrefab, contentUI.transform).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = drop.item.name + " [" + drop.amount.ToString() + "]";
        }
        DCButtonIdentity id = buttonText.GetComponentInParent<DCButtonIdentity>();
        if (id != null)
        {
            id.drop = drop;
        }
    }

    public void TakeAll()
    {
       if (contentUI.activeSelf)
        {
            int count = contentUI.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                for (int a = 0; a < contentUI.transform.GetChild(i).GetComponent<DCButtonIdentity>().drop.amount; a++)
                    {
                        contentUI.transform.GetChild(i).GetComponent<DCButtonIdentity>().AddSingle();
                    }
            }
        }
    }



}
