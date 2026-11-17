using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PotController : MonoBehaviour
{
    [Header("Receta (solo para probar esta escena sola)")]
    [SerializeField] private RecipeData defaultRecipe;

    [Header("UI")]
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private GameObject undoButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private MilkLevel milkLevel;

    [Header("Navegación")]
    [SerializeField] private string kitchenScene = "Cocina";

    private RecipeData recipe;
    private Challenge current;
    private DraggableMeasure[] measures;
    private readonly Stack<int> pours = new Stack<int>();
    private int challengeIndex;
    private int currentAmount;
    private int hintLevel;
    private bool completed;

    private void Start()
    {
        recipe = RecipeSession.Current != null ? RecipeSession.Current : defaultRecipe;

        if (recipe == null || recipe.challenges == null || recipe.challenges.Length == 0)
        {
            Debug.LogWarning("PotController: la receta no tiene retos.", this);
            return;
        }

        measures = FindObjectsByType<DraggableMeasure>(FindObjectsSortMode.None);
        StartChallenge(0);
    }

    private string A(int units) { return QuantityFormat.Amount(units, current); }

    private string NowText()
    {
        if (current.displayMode == DisplayMode.Fraction)
            return QuantityFormat.FractionWithEquivalent(currentAmount, current.unitsPerWhole) + " L";
        return A(currentAmount);
    }

    private void StartChallenge(int index)
    {
        challengeIndex = index;
        current = recipe.challenges[index];
        currentAmount = current.startMl;
        hintLevel = 0;
        completed = false;
        pours.Clear();

        if (promptText != null) promptText.text = current.prompt;
        if (nextButton != null) nextButton.SetActive(false);

        foreach (var m in measures)
        {
            int amount = (current.measureValues != null && m.Slot < current.measureValues.Length)
                ? current.measureValues[m.Slot]
                : m.DefaultAmount;

            m.SetAmount(amount, QuantityFormat.Short(amount, current));

            bool enabled = current.enabledMeasures == null
                || current.enabledMeasures.Length == 0
                || System.Array.IndexOf(current.enabledMeasures, amount) >= 0;

            m.SetEnabled(enabled);
            m.SetHighlight(false);
        }

        UpdateUI();
        ShowMessage($"Meta: {A(current.targetMl)}. Ahora hay {A(currentAmount)}.");
    }

    public bool CanPour(int amount)
    {
        if (completed || current == null) return false;
        return current.allowOverflow || currentAmount + amount <= current.targetMl;
    }

    public bool TryPour(int amount)
    {
        if (completed || current == null) return false;

        int missing = current.targetMl - currentAmount;

        if (!current.allowOverflow && amount > missing)
        {
            ShowMessage($"¡Te pasas! Ese recipiente tiene {A(amount)}.\n{BuildHint()}");
            return false;
        }

        currentAmount += amount;
        pours.Push(amount);
        hintLevel = 0;
        ClearHighlights();
        UpdateUI();

        if (currentAmount == current.targetMl)
        {
            Complete();
        }
        else if (currentAmount > current.targetMl)
        {
            int excess = currentAmount - current.targetMl;
            ShowMessage($"¡Te pasaste por {A(excess)}! Echaste {A(amount)} y solo faltaba {A(missing)}. Pulsa Deshacer para corregir.");
        }
        else
        {
            ShowMessage($"Vas {NowText()}. Faltan {A(current.targetMl - currentAmount)}.");
        }

        return true;
    }

    public void Undo()
    {
        if (completed || pours.Count == 0) return;

        int last = pours.Pop();
        currentAmount -= last;
        hintLevel = 0;
        ClearHighlights();
        UpdateUI();
        ShowMessage($"Sacaste {A(last)}. Hay {A(currentAmount)}; faltan {A(Mathf.Max(0, current.targetMl - currentAmount))}.");
    }

    public void Hint()
    {
        if (current == null || completed) return;
        ShowMessage(BuildHint());
    }

    public void OnNextClicked()
    {
        bool last = challengeIndex >= recipe.challenges.Length - 1;
        if (last) SceneManager.LoadScene(kitchenScene);
        else StartChallenge(challengeIndex + 1);
    }

    private string BuildHint()
    {
        int missing = current.targetMl - currentAmount;

        if (missing < 0)
            return "Te pasaste. Pulsa Deshacer y prueba con otro recipiente.";

        hintLevel++;

        if (hintLevel == 1)
            return $"Pista: faltan {A(missing)} para llegar a {A(current.targetMl)}.";

        if (hintLevel == 2)
            return $"Pista: busca un recipiente de {A(missing)} o menos, o combina dos.";

        foreach (var m in measures)
            m.SetHighlight(m.Interactable && m.Amount <= missing);

        return "Pista: los recipientes que brillan caben en la olla.";
    }

    private void ClearHighlights()
    {
        foreach (var m in measures) m.SetHighlight(false);
    }

    private void Complete()
    {
        completed = true;
        bool last = challengeIndex >= recipe.challenges.Length - 1;

        if (last)
        {
            bool firstTime = !ProgressManager.IsCompleted(recipe);
            ProgressManager.MarkCompleted(recipe);
            if (firstTime) RecipeSession.JustCompleted = recipe;
        }

        ShowMessage(last
            ? "¡Receta completada!"
            : $"¡Lo lograste! {A(current.targetMl)} exactos.");

        if (nextButton != null) nextButton.SetActive(true);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (counterText != null)
            counterText.text = QuantityFormat.Counter(currentAmount, current);

        if (milkLevel != null)
            milkLevel.SetFill((float)currentAmount / current.targetMl);

        if (undoButton != null)
            undoButton.SetActive(pours.Count > 0 && !completed);
    }

    private void ShowMessage(string msg)
    {
        if (messageText != null) messageText.text = msg;
    }
}