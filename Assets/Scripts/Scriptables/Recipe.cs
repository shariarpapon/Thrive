using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Scriptables/Recipes/New Recipe")]
public class Recipe : ScriptableObject
{
    public string recipeName;
    [Space]
    public MaterialSet[] inputs;
    [Space]
    public Item output;
    public int outputAmount;
}
