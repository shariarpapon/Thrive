using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeButtonEvent : MonoBehaviour
{
    public Recipe recipe;
    private GameObject craftingDisplay;
    private InventoryManager inv_manager;
    private CraftingManager craftingManager;

    private void Awake()
    {
        craftingDisplay = GameObject.FindGameObjectWithTag("CraftingDisplay");
        inv_manager = FindObjectOfType<InventoryManager>();
        craftingManager = FindObjectOfType<CraftingManager>();
    }

    public void OnRecipeSelected()
    {
        if (recipe != null)
        {
            FindObjectOfType<InventoryButtonEvents>().activeRecipeButton = this;
            TextMeshProUGUI inText = craftingDisplay.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI outText = craftingDisplay.transform.GetChild(3).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            Image icon = craftingDisplay.transform.GetChild(3).transform.GetChild(0).GetComponent<Image>();
            icon.sprite = recipe.output.icon;
            outText.text = recipe.outputAmount.ToString() + "x " + recipe.output.name;
            string inputMats = string.Empty;

            for (int i = 0; i < recipe.inputs.Length; i++)
            {
                if (i > 0)
                    inputMats += "\n" + "[ " + inv_manager.GetItemCount(recipe.inputs[i].item) + " ] " + recipe.inputs[i].amount + "x " + recipe.inputs[i].item.name;
                else if (i == 0)
                    inputMats += "[ " + inv_manager.GetItemCount(recipe.inputs[i].item) + " ] " + recipe.inputs[i].amount + "x " + recipe.inputs[i].item.name;
            }
            inText.text = inputMats;

            bool hasEnoughItems = true;
            foreach (MaterialSet mat in recipe.inputs)
            {
                int count = inv_manager.GetItemCount(mat.item);
                if (count < mat.amount)
                    hasEnoughItems = false;
            }
            if (!hasEnoughItems)
            {
                craftingDisplay.transform.GetChild(4).gameObject.SetActive(true);
            }
            else
                craftingDisplay.transform.GetChild(4).gameObject.SetActive(false);

            craftingManager.activeRecipe = recipe;
        }
    }
}
