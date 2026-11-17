using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameModeManager : MonoBehaviour
{
    [SerializeField] private GameObject recipePanel;
    [SerializeField] private GameObject bookButton;
    [SerializeField] private string preparationScene = "PreparacionArrozConLeche";

    [Header("Progreso")]
    [SerializeField] private RecipeCatalog catalog;
    [SerializeField] private TMP_Text noticeText;

    private void Start()
    {
        ShowRecipeBook(false);
        ShowUnlockNotice();
    }

    public void OpenRecipeBook() { ShowRecipeBook(true); }
    public void CloseRecipeBook() { ShowRecipeBook(false); }

    private void ShowRecipeBook(bool show)
    {
        recipePanel.SetActive(show);
        bookButton.SetActive(!show);
    }

    public void StartRecipe(RecipeData recipe)
    {
        RecipeSession.Current = recipe;
        string scene = string.IsNullOrEmpty(recipe.sceneName) ? preparationScene : recipe.sceneName;
        SceneManager.LoadScene(scene);
    }

    private void ShowUnlockNotice()
    {
        if (noticeText != null) noticeText.gameObject.SetActive(false);

        RecipeData done = RecipeSession.JustCompleted;
        RecipeSession.JustCompleted = null;

        if (done == null || catalog == null || noticeText == null) return;

        int next = catalog.IndexOf(done) + 1;
        if (next <= 0 || next >= catalog.recipes.Length) return;

        noticeText.text = $"¡Receta desbloqueada: {catalog.recipes[next].title}!";
        noticeText.gameObject.SetActive(true);
        StartCoroutine(HideNotice(4f));
    }

    private IEnumerator HideNotice(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        noticeText.gameObject.SetActive(false);
    }

    // Para pruebas: menú ⋮ del componente > "Borrar progreso guardado"
    [ContextMenu("Borrar progreso guardado")]
    private void ResetProgress()
    {
        if (catalog != null) ProgressManager.ResetAll(catalog);
        Debug.Log("Progreso borrado.");
    }
}