using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryButtonEvents : MonoBehaviour
{
    public GameObject slotMenuPanel;
    public GameObject removePanel;
    public GameObject craftMenuPanel;
    public GameObject craftingDisplay;
    [HideInInspector]
    public RecipeButtonEvent activeRecipeButton;
    private Slot target;

    private void Awake()
    {
        InitializeSlotMenu();
    }

    #region Slot Menu Events
    private void InitializeSlotMenu()
    {
        removePanel.transform.GetChild(1).GetComponent<TMP_InputField>().text = 1.ToString();
        removePanel.SetActive(false);
        slotMenuPanel.SetActive(false);
        slotMenuPanel.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
    }

    public void OnSlotMenu(Slot slot)
    {
        Item item = slot.slotItem;
        if (item != null)
        {
            transform.SetAsLastSibling();
            target = slot;
            slotMenuPanel.SetActive(true);
            slotMenuPanel.transform.position = Input.mousePosition;
            slotMenuPanel.transform.GetChild(2).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = target.slotItem.description;
            removePanel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = target.itemCount.ToString();
        }
    }

    public void OnSlotMenuClose()
    {
        target = null;
        OnRemoveClose();
        OnDescriptionClose();
        slotMenuPanel.SetActive(false);
    }

    public void ResetCraftingDisplay()
    {
        if (activeRecipeButton != null)
            activeRecipeButton.OnRecipeSelected();
    }

    public void OnUse()
    {
        if (target != null)
        {
            target.UseItem();
        }
    }

    public void OnDescription()
    {
        if (target != null)
        {
            OnRemoveClose();
            GameObject panel = slotMenuPanel.transform.GetChild(2).transform.GetChild(1).gameObject;
            panel.SetActive(true);
            slotMenuPanel.transform.GetChild(2).GetComponent<Button>().interactable = false;
            panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = target.slotItem.description;
        }
    }

    public void OnDescriptionClose()
    {
        GameObject panel = slotMenuPanel.transform.GetChild(2).transform.GetChild(1).gameObject;
        slotMenuPanel.transform.GetChild(2).GetComponent<Button>().interactable = true;
        panel.SetActive(false);
    }

    public void OnRemove()
    {
        OnDescriptionClose();
        removePanel.SetActive(true);
        removePanel.transform.parent.transform.GetComponent<Button>().interactable = false;
        removePanel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = target.itemCount.ToString();
    }

    public void OnRemoveClose()
    {
        removePanel.transform.parent.transform.GetComponent<Button>().interactable = true;
        removePanel.transform.GetChild(1).GetComponent<TMP_InputField>().text = string.Empty;
        removePanel.SetActive(false);
    }

    public void OnRemoveInputButton()
    {
        string input = removePanel.transform.GetChild(1).GetComponent<TMP_InputField>().text;
        int amount;

        if (input == string.Empty)
            amount = 1;
        else
            amount = int.Parse(input);

        TextMeshProUGUI tmp = removePanel.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        if (target.RemoveItem(amount))
        {
            tmp.text = target.itemCount.ToString();
            OnSlotMenuClose();
            return;
        }
        tmp.text = target.itemCount.ToString();
    }
    #endregion


}
