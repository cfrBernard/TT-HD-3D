using UnityEngine;
using UnityEngine.InputSystem;

public class CardDragHandler : MonoBehaviour
{
    private Camera cam;
    private CardView cardView;
    private bool dragging;

    [SerializeField] private LayerMask cardLayerMask;
    [SerializeField] private LayerMask slotLayerMask;

    void Awake()
    {
        cam = Camera.main;
        cardView = GetComponent<CardView>();
    }

    void Update()
    {
        // clic gauche down -> vérifier si on a cliqué sur CETTE carte
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, cardLayerMask))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    dragging = true;
                }
            }
        }

        // clic gauche up -> vérifier si on a relaché sur CE slot
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
                        Debug.Log($"Card '{cardView.Card.Data.name}' placed at {slotView.Slot.X},{slotView.Slot.Y}");
                    }
                    else
                    {
                        // rollback
                    }
                }
            }
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
