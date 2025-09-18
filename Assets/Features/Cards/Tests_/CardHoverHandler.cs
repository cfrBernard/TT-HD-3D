using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(CardView))]
public class CardHoverHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask cardLayerMask;
    [SerializeField] private float zoomScale = 1.2f;
    [SerializeField] private float neighborOffset = 0.3f;
    [SerializeField] private float animDuration = 0.2f;
    [SerializeField] private float maxTilt = 10f;

    private Camera cam;
    private CardView cardView;
    private HandLayout handLayout;
    private Transform slot;
    private int slotIndex;

    private bool isHover = false;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Vector3[] slotOriginalPositions;

    void Awake()
    {
        cam = Camera.main;
        cardView = GetComponent<CardView>();
        slot = transform.parent;
        handLayout = slot.GetComponentInParent<HandLayout>();

        originalScale = transform.localScale;
        originalRotation = transform.localRotation;

        // Sauvegarde des pos initiales des slots
        slotOriginalPositions = new Vector3[handLayout.slots.Length];
        for (int i = 0; i < handLayout.slots.Length; i++)
            slotOriginalPositions[i] = handLayout.slots[i].localPosition;

        Debug.Log($"[CardHoverHandler] Awake -> card='{cardView.Card?.Data.name}' slots={handLayout.slots.Length}");
    }

    void Update()
    {
        // Check hover via raycast
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, cardLayerMask))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (!isHover)
                {
                    Debug.Log($"[CardHoverHandler] Hover Enter -> {cardView.Card.Data.name}");
                    EnterHover();
                }

                UpdateHoverTilt();
                return;
            }
        }

        if (isHover)
        {
            Debug.Log($"[CardHoverHandler] Hover Exit -> {cardView.Card.Data.name}");
            ExitHover();
        }
    }

    private void EnterHover()
    {
        isHover = true;
        slotIndex = System.Array.IndexOf(handLayout.slots, slot);

        Debug.Log($"[CardHoverHandler] EnterHover slotIndex={slotIndex}");

        // Zoom
        transform.DOScale(originalScale * zoomScale, animDuration);

        // Décale voisins
        OffsetNeighbor(slotIndex - 1, -neighborOffset);
        OffsetNeighbor(slotIndex + 1, neighborOffset);

        // FX / SFX
        // cardView.PlayHoverFx();
        // cardView.PlayHoverSfx();
    }

    private void ExitHover()
    {
        isHover = false;

        Debug.Log($"[CardHoverHandler] ExitHover slotIndex={slotIndex}");

        // Reset scale & rotation
        transform.DOScale(originalScale, animDuration);
        transform.DOLocalRotateQuaternion(originalRotation, animDuration);

        // Reset slots
        ResetSlot(slotIndex - 1);
        ResetSlot(slotIndex + 1);
    }

    private void UpdateHoverTilt()
    {
        Vector2 mouse = Mouse.current.position.ReadValue();
        Vector2 screenPos = cam.WorldToScreenPoint(transform.position);
        Vector2 delta = (mouse - screenPos) / 100f;

        float rotX = Mathf.Clamp(-delta.y * maxTilt, -maxTilt, maxTilt);
        float rotY = Mathf.Clamp(delta.x * maxTilt, -maxTilt, maxTilt);

        transform.localRotation = Quaternion.Euler(rotX, rotY, 0f);

        Debug.Log($"[CardHoverHandler] Tilt -> rotX={rotX:F2} rotY={rotY:F2}");
    }

    private void OffsetNeighbor(int idx, float offset)
    {
        if (idx < 0 || idx >= handLayout.slots.Length) return;
        var slotT = handLayout.slots[idx];

        Debug.Log($"[CardHoverHandler] OffsetNeighbor idx={idx} offset={offset}");

        // Move avec DO
        slotT.DOLocalMoveX(slotOriginalPositions[idx].x + offset, animDuration);
    }

    private void ResetSlot(int idx)
    {
        if (idx < 0 || idx >= handLayout.slots.Length) return;
        var slotT = handLayout.slots[idx];

        Debug.Log($"[CardHoverHandler] ResetSlot idx={idx}");

        // Reviens à la pos d’origine
        slotT.DOLocalMove(slotOriginalPositions[idx], animDuration);
    }
}
