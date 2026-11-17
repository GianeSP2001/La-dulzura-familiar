using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableMeasure : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Identificación")]
    [SerializeField] private int slot = 0;          // Vaso100 = 0, Vaso200 = 1, Vaso300 = 2, Vaso400 = 3
    [SerializeField] private int amountMl = 200;    // valor original (nivel de mililitros)
    [SerializeField] private TMP_Text label;        // texto sobre el vaso (opcional)

    [Header("Referencias")]
    [SerializeField] private PotController pot;
    [SerializeField] private Collider2D potCollider;

    [Header("Animación de verter")]
    [SerializeField] private float pourAngle = 0f;
    [SerializeField] private float tiltTime = 0.3f;
    [SerializeField] private float holdTime = 0.5f;
    [SerializeField] private Vector2 pourOffset = new Vector2(-0.6f, 0.8f);

    [Header("Sprite de verter (opcional)")]
    [SerializeField] private Sprite pouringSprite;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Camera cam;
    private SpriteRenderer sr;
    private Sprite normalSprite;
    private Color baseColor = Color.white;
    private int currentAmount;
    private bool busy;
    private bool interactable = true;
    private bool highlighted;

    public int Slot => slot;
    public int DefaultAmount => amountMl;
    public int Amount => currentAmount;
    public bool Interactable => interactable;

    private void Awake()
    {
        cam = Camera.main;
        startPosition = transform.position;
        startRotation = transform.rotation;
        currentAmount = amountMl;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            baseColor = sr.color;
            normalSprite = sr.sprite;
        }
    }

    // La llama PotController al empezar cada reto
    public void SetAmount(int amount, string labelText)
    {
        currentAmount = amount;
        if (label != null) label.text = labelText;
    }

    public void SetEnabled(bool on) { interactable = on; Refresh(); }
    public void SetHighlight(bool on) { highlighted = on; Refresh(); }

    private void Refresh()
    {
        if (sr == null) return;
        Color c = highlighted ? new Color(1f, 0.92f, 0.4f) : baseColor;
        c.a = interactable ? 1f : 0.35f;
        sr.color = c;
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData)
    {
        if (busy || !interactable) return;
        Vector3 world = cam.ScreenToWorldPoint(eventData.position);
        world.z = startPosition.z;
        transform.position = world;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (busy || !interactable) return;

        Vector3 world = cam.ScreenToWorldPoint(eventData.position);
        if (potCollider.OverlapPoint(world))
            StartCoroutine(PourRoutine());
        else
            StartCoroutine(ReturnRoutine());
    }

    private IEnumerator PourRoutine()
    {
        busy = true;

        Vector3 pourPos = potCollider.bounds.center + (Vector3)pourOffset;
        pourPos.z = startPosition.z;
        yield return MoveTo(pourPos, 0.15f);

        bool useSprite = pouringSprite != null && sr != null;
        bool useRotation = !useSprite && !Mathf.Approximately(pourAngle, 0f);

        if (pot.CanPour(currentAmount))
        {
            if (useSprite)
            {
                sr.sprite = pouringSprite;
                yield return new WaitForSeconds(0.1f);
            }
            else if (useRotation)
            {
                yield return RotateTo(startRotation * Quaternion.Euler(0, 0, pourAngle), tiltTime);
            }

            pot.TryPour(currentAmount);
            yield return new WaitForSeconds(holdTime);
        }
        else
        {
            pot.TryPour(currentAmount);
            yield return Shake(0.3f);
        }

        if (useSprite) sr.sprite = normalSprite;
        else if (useRotation) yield return RotateTo(startRotation, tiltTime);

        yield return MoveTo(startPosition, 0.25f);
        busy = false;
    }

    private IEnumerator ReturnRoutine()
    {
        busy = true;
        yield return MoveTo(startPosition, 0.2f);
        busy = false;
    }

    private IEnumerator MoveTo(Vector3 target, float time)
    {
        Vector3 from = transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / time;
            transform.position = Vector3.Lerp(from, target, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        transform.position = target;
    }

    private IEnumerator RotateTo(Quaternion target, float time)
    {
        Quaternion from = transform.rotation;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / time;
            transform.rotation = Quaternion.Slerp(from, target, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        transform.rotation = target;
    }

    private IEnumerator Shake(float duration)
    {
        Vector3 origin = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = origin + new Vector3(Mathf.Sin(elapsed * 60f) * 0.08f, 0, 0);
            yield return null;
        }
        transform.position = origin;
    }
}