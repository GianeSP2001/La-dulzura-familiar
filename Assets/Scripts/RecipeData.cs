using UnityEngine;

public enum DisplayMode { Milliliters, Fraction }
public enum TimeMode { SetMinutes, ChooseOption }

// Reto de la olla. Las cantidades están en "unidades":
// mililitros en el nivel 1, octavos de litro en el nivel 2 (modo Fraction).
[System.Serializable]
public class Challenge
{
    [TextArea(1, 3)] public string prompt;
    public int startMl = 0;
    public int targetMl = 500;
    public bool allowOverflow = false;
    public int[] enabledMeasures = { 100, 200, 300, 400 };   // vacío = todos activos

    public DisplayMode displayMode = DisplayMode.Milliliters;
    public int unitsPerWhole = 8;     // solo fracciones: 8 unidades = 1 litro
    public int[] measureValues;       // valor de cada vaso por posición (0 a 3). Vacío = valores originales
}

// Reto de tiempo (nivel 3)
[System.Serializable]
public class TimeChallenge
{
    [TextArea(1, 3)] public string prompt;
    public TimeMode mode = TimeMode.SetMinutes;

    // SetMinutes: el objetivo son 60 * numerator / denominator minutos
    public int numerator = 1;
    public int denominator = 2;

    // ChooseOption: opciones y cuál es la correcta
    public string[] options;
    public int correctIndex;

    [TextArea(1, 3)] public string explanation;   // se muestra al acertar
    public string[] hints;                        // opcional; si está vacío usa pistas automáticas
}

[CreateAssetMenu(fileName = "Receta", menuName = "Dulceria/Receta")]
public class RecipeData : ScriptableObject
{
    public string id = "arroz-con-leche";
    public string title = "Arroz con leche";
    [TextArea(2, 5)] public string story;
    public string[] ingredients;
    [TextArea(2, 5)] public string challenge;
    public int targetMl = 500;
    public Challenge[] challenges;

    [Header("Opcional")]
    public string sceneName;                 // escena propia (vacío = escena de preparación)
    public TimeChallenge[] timeChallenges;   // retos de tiempo

    public bool HasChallenges =>
        (challenges != null && challenges.Length > 0) ||
        (timeChallenges != null && timeChallenges.Length > 0);
}