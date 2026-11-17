using System.Collections;
using UnityEngine;

public class MilkLevel : MonoBehaviour
{
    [SerializeField] private SpriteRenderer target;   // el SpriteRenderer del bol (la Olla)
    [SerializeField] private Sprite[] stages;         // de vacío a lleno
    [SerializeField] private Sprite overflowSprite;   // bol derramado

    [Header("Efecto al cambiar (pon 1 para desactivarlo)")]
    [SerializeField] private float popScale = 1.08f;
    [SerializeField] private float popTime = 0.15f;

    private Sprite currentSprite;
    private Vector3 baseScale;

    private void Awake()
    {
        if (target == null) target = GetComponent<SpriteRenderer>();
        baseScale = target.transform.localScale;
    }

    // 0 = vacío, 1 = meta exacta, mayor que 1 = se pasó
    public void SetFill(float normalized)
    {
        Sprite next = Pick(normalized);
        if (next == null || next == currentSprite) return;

        bool first = currentSprite == null;
        currentSprite = next;
        target.sprite = next;

        if (!first && popScale > 1f && isActiveAndEnabled)
        {
            StopAllCoroutines();
            target.transform.localScale = baseScale;
            StartCoroutine(Pop());
        }
    }

    private Sprite Pick(float f)
    {
        if (stages == null || stages.Length == 0) return null;
        if (f > 1f && overflowSprite != null) return overflowSprite;

        int n = stages.Length;
        if (f <= 0f) return stages[0];
        if (f >= 1f) return stages[n - 1];
        if (n < 3) return stages[0];

        // Con algo de leche ya sale al menos la primera etapa con leche
        int index = 1 + Mathf.FloorToInt(f * (n - 2));
        return stages[Mathf.Clamp(index, 1, n - 2)];
    }

    private IEnumerator Pop()
    {
        Transform t = target.transform;
        float half = popTime / 2f;

        float time = 0f;
        while (time < half)
        {
            time += Time.deltaTime;
            t.localScale = Vector3.Lerp(baseScale, baseScale * popScale, time / half);
            yield return null;
        }

        time = 0f;
        while (time < half)
        {
            time += Time.deltaTime;
            t.localScale = Vector3.Lerp(baseScale * popScale, baseScale, time / half);
            yield return null;
        }

        t.localScale = baseScale;
    }
}