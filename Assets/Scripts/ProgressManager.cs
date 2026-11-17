using UnityEngine;

public static class ProgressManager
{
    private const string Prefix = "receta_completada_";

    public static bool IsCompleted(RecipeData recipe)
    {
        return PlayerPrefs.GetInt(Prefix + recipe.id, 0) == 1;
    }

    public static void MarkCompleted(RecipeData recipe)
    {
        PlayerPrefs.SetInt(Prefix + recipe.id, 1);
        PlayerPrefs.Save();
    }

    public static void ResetAll(RecipeCatalog catalog)
    {
        foreach (var r in catalog.recipes)
            PlayerPrefs.DeleteKey(Prefix + r.id);
        PlayerPrefs.Save();
    }
}