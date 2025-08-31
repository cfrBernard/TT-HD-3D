using UnityEngine;
using UnityEngine.InputSystem;

public class CardDragHandler : MonoBehaviour
{
    [SerializeField] private LayerMask cardLayerMask;
    [SerializeField] private LayerMask slotLayerMask;

    private Camera cam;
    private CardView cardView;
    private bool dragging;
    private bool isPlaced = false;

    // Rollback
    private Vector3 originalPos;
    private Quaternion originalRot;

    void Awake()
    {
        cam = Camera.main;
        cardView = GetComponent<CardView>();
    }

    void Update()
    {
        // clic gauche down -> start drag
        if (Mouse.current.leftButton.wasPressedThisFrame && !isPlaced)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, cardLayerMask))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    originalPos = transform.position;
                    originalRot = transform.rotation;
                    dragging = true;
                }
            }
        }

        // clic gauche up -> drop
        if (Mouse.current.leftButton.wasReleasedThisFrame && dragging)
        {
            dragging = false;

            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, slotLayerMask))
            {
                BoardSlotView slotView = hit.collider.GetComponent<BoardSlotView>();
                if (slotView != null && slotView.IsEmpty)
                {
                    bool placed = FindFirstObjectByType<BoardManager>()
                        .TryPlaceCard(slotView.Slot.X, slotView.Slot.Y, cardView.Card);

                    if (placed)
                    {
                        transform.position = slotView.transform.position;
                        isPlaced = true;
                        Debug.Log($"Card '{cardView.Card.Data.name}' placed at {slotView.Slot.X},{slotView.Slot.Y}");
                        return; // fin -> pas de rollback
                    }
                }
            }

            // rollback si pas placé
            transform.position = originalPos;
            transform.rotation = originalRot;
        }

        // drag en cours
        if (dragging)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, ~cardLayerMask)) // ignore les cartes
            {
                transform.position = hit.point + Vector3.up * 0.1f;
            }
        }
    }
}
