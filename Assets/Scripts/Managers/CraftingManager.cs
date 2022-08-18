using UnityEngine;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class MaterialSet
{
    public Item item;
    public int amount;
}

public class CraftingManager : MonoBehaviour
{
    public InventoryButtonEvents inventoryButtonEvents;
    public InventoryManager inventoryManager;
    public Recipe[] recipes;
    [HideInInspector]
    public Recipe activeRecipe;
    public List<string> recipeNames = new List<string>();

    private void Awake()
    {
        Thread thread = new Thread(AssignRecipeNames);
        thread.Start();
    }

    private void AssignRecipeNames()
    {
        foreach (Recipe r in recipes)
        {
            recipeNames.Add(r.recipeName);
        }
    }

    public void CraftItem()
    {
        if (activeRecipe != null)
        {
            if (ContainSufficientItems())
            {
                if (inventoryManager.HasEnoughSpace(activeRecipe.output, activeRecipe.outputAmount))
                {
                    foreach (MaterialSet mat in activeRecipe.inputs)
                    {
                        Slot s = inventoryManager.GetSlotWithItem(mat.item, mat.amount);
                        if (s != null)
                            s.RemoveItem(mat.amount);
                        else
                            return;
                    }
                    for (int i = 0; i < activeRecipe.outputAmount; i++)
                    {
                        inventoryManager.AddToInventory(activeRecipe.output);
                    }
                    inventoryButtonEvents.ResetCraftingDisplay();
                    AudioManager.instance.InstantPlay(AudioManager.instance.craftingButtonSFX);
                }
                if (!ContainSufficientItems())
                    GameObject.FindGameObjectWithTag("CraftingDisplay").transform.GetChild(4).gameObject.SetActive(true);
            } 
        }
    }

    private bool ContainSufficientItems()
    {
        foreach (MaterialSet mat in activeRecipe.inputs)
        {
            int count = inventoryManager.GetItemCount(mat.item);
            if (count < mat.amount)
                return false;
        }
        return true;
    }





}


