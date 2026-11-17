using UnityEngine;
using TMPro;

public class RecipeBookUI : MonoBehaviour
{
    [SerializeField] private RecipeCatalog catalog;
    [SerializeField] private GameModeManager manager;

    [Header("Textos")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text storyText;
    [SerializeField] private TMP_Text ingredientsText;
    [SerializeField] private TMP_Text challengeText;
    [SerializeField] private TMP_Text lockText;

    [Header("Objetos")]
    [SerializeField] private GameObject contentGroup;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private GameObject nextButton;

    private int page;

    private void OnEnable()
    {
        if (catalog == null || catalog.recipes == null || catalog.recipes.Length == 0)
        {
            Debug.LogWarning("RecipeBookUI: falta el catálogo o está vacío.", this);
            return;
        }

        page = FirstPendingPage();
        Show();
    }

    public void NextPage() { ChangePage(1); }
    public void PrevPage() { ChangePage(-1); }

    private void ChangePage(int delta)
    {
        page = Mathf.Clamp(page + delta, 0, catalog.recipes.Length - 1);
        Show();
    }

    // Abre el libro en la primera receta desbloqueada que aún no se completó
    private int FirstPendingPage()
    {
        for (int i = 0; i < catalog.recipes.Length; i++)
            if (catalog.IsUnlocked(i) && !ProgressManager.IsCompleted(catalog.recipes[i]))
                return i;
        return 0;
    }

    private void Show()
    {
        RecipeData r = catalog.recipes[page];
        bool unlocked = catalog.IsUnlocked(page);
        bool playable = r.HasChallenges;
        bool done = ProgressManager.IsCompleted(r);

        titleText.text = done ? r.title + " (completada)" : r.title;

        contentGroup.SetActive(unlocked);
        lockText.gameObject.SetActive(!unlocked);

        if (unlocked)
        {
            storyText.text = r.story;
            ingredientsText.text = (r.ingredients != null && r.ingredients.Length > 0)
                ? "• " + string.Join("\n• ", r.ingredients)
                : "";
            challengeText.text = playable ? r.challenge : "¡Próximamente!";
        }
        else
        {
            lockText.text = $"Bloqueada\nCompleta «{catalog.recipes[page - 1].title}» para desbloquearla.";
        }

        startButton.SetActive(unlocked && playable);
        prevButton.SetActive(page > 0);
        nextButton.SetActive(page < catalog.recipes.Length - 1);
    }

    public void OnStartClicked() { manager.StartRecipe(catalog.recipes[page]); }
    public void OnCloseClicked() { manager.CloseRecipeBook(); }
}