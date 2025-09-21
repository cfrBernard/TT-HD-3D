using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(CardView))]
public class CardHoverHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask cardLayerMask;
    [SerializeField] private float hoverOffsetZ = -50f;
    [SerializeField] private float neighborOffset = 10f;
    [SerializeField] private float animDuration = 0.2f;
    [SerializeField] private float maxTilt = 100f;

    private Camera cam;
    private CardView cardView;
    private HandLayout handLayout;
    private Transform slot;
    private int slotIndex;

    private bool isHover = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3[] slotOriginalPositions;

    void Awake()
    {
        cam = Camera.main;
        cardView = GetComponent<CardView>();
    }

    public void InitHover(HandLayout layout, Transform targetSlot)
    {
        handLayout = layout;
        slot = targetSlot;

        if (handLayout == null || slot == null)
        {
            Debug.LogError($"[CardHoverHandler] InitHover échoué pour '{cardView.Card?.Data.name}' → layout ou slot null.");
            return;
        }

        // force 0,0,0 (pos/rotate)
        originalPosition = Vector3.zero;
        originalRotation = Quaternion.identity; 

        // Sauvegarde des pos initiales des slots
        slotOriginalPositions = new Vector3[handLayout.slots.Length];
        for (int i = 0; i < handLayout.slots.Length; i++)
            slotOriginalPositions[i] = handLayout.slots[i].localPosition;

        Debug.Log($"[CardHoverHandler] InitHover -> card='{cardView.Card?.Data.name}' slot={slot.name} slots={handLayout.slots.Length}");
    }

    void Update()
    {
        if (handLayout == null) return; // pas encore init

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

                // UpdateHoverTilt(); --- TESTING ---
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

        // Décalage Z au hover (~zoom) --- TESTING ---
        // transform.DOLocalMoveZ(originalPosition.z + hoverOffsetZ, animDuration);

        // Décale voisins
        // OffsetNeighbor(slotIndex - 1, -neighborOffset); --- TESTING ---
        OffsetNeighbor(slotIndex + 1, neighborOffset);

        // FX placeholder
        cardView.EnableHoverFX();
    }

    private void ExitHover()
    {
        isHover = false;

        Debug.Log($"[CardHoverHandler] ExitHover slotIndex={slotIndex}");

        // Revenir à la position originale
        transform.DOLocalMove(originalPosition, animDuration);
        transform.DOLocalRotateQuaternion(originalRotation, animDuration);

        // Reset slots
        ResetSlot(slotIndex - 1);
        ResetSlot(slotIndex + 1);
    }

    //private void UpdateHoverTilt() --- TESTING ---
    //{
    //    if (slot == null) return;
    //
    //    Vector2 mouse = Mouse.current.position.ReadValue();
    //    Vector2 screenPos = cam.WorldToScreenPoint(slot.position);
    //    Vector2 delta = (mouse - screenPos) / 100f;
    //
    //    float rotX = Mathf.Clamp(-delta.y * maxTilt, -maxTilt, maxTilt);
    //    float rotY = Mathf.Clamp(delta.x * maxTilt, -maxTilt, maxTilt);
    //
    //    slot.localRotation = Quaternion.Euler(rotX, rotY, 0f);
    //}

    private void OffsetNeighbor(int idx, float offset)
    {
        if (idx < 0 || idx >= handLayout.slots.Length) return;
        var slotT = handLayout.slots[idx];

        slotT.DOLocalMoveY(slotOriginalPositions[idx].y + offset, animDuration);
    }

    private void ResetSlot(int idx)
    {
        if (idx < 0 || idx >= handLayout.slots.Length) return;
        var slotT = handLayout.slots[idx];

        slotT.DOLocalMoveY(slotOriginalPositions[idx].y, animDuration);
        slot.DOLocalRotateQuaternion(Quaternion.identity, animDuration);

        // FX placeholder
        cardView.DisableHoverFX();
    }
}
