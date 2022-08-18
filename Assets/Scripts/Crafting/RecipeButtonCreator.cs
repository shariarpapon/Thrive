using UnityEngine;
using UnityEngine.UI;

public class RecipeButtonCreator : MonoBehaviour
{
    public GameObject recipeButton;
    public Transform buttonParent;

    private CraftingManager craftingManager;

    private void Awake()
    {
        craftingManager = FindObjectOfType<CraftingManager>();
    }

    private void Start()
    {
        for (int i = 0; i < craftingManager.recipes.Length; i++)
        {
            GameObject button = Instantiate(recipeButton, buttonParent) as GameObject;
            button.transform.GetChild(0).GetComponent<Text>().text = craftingManager.recipes[i].recipeName;
            button.GetComponent<RecipeButtonEvent>().recipe = craftingManager.recipes[i];
        }
    }

 
}
