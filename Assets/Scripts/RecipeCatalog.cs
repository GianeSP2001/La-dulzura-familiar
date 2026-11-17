using UnityEngine;

[CreateAssetMenu(fileName = "Catalogo", menuName = "Dulceria/Catalogo")]
public class RecipeCatalog : ScriptableObject
{
    public RecipeData[] recipes;

    public bool IsUnlocked(int index)
    {
        return index <= 0 || ProgressManager.IsCompleted(recipes[index - 1]);
    }

    public int IndexOf(RecipeData recipe)
    {
        return System.Array.IndexOf(recipes, recipe);
    }
}