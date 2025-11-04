using UnityEngine;
using UnityEngine.InputSystem;

public class CardClickHandler : MonoBehaviour
{
    public static event System.Action<Card> OnCardClicked;

    public static bool MulliganActive = false;   // activé par le MatchManager
    public static Player CurrentPlayer;          // joueur qui peut cliquer

    [SerializeField] private LayerMask cardLayerMask;

    private Camera cam;
    private CardView cardView;

    private bool highlighted = false;

    void Awake()
    {
        cam = Camera.main;
        cardView = GetComponent<CardView>();
    }

    void Update()
    {
        if (!MulliganActive) return;

        // Vérifie que c'est bien une carte du current player
        if (cardView.Card.Owner != CurrentPlayer) return;

        // Highlight placeholder
        if (!highlighted)
        {
            Highlight(true);
            highlighted = true;
        }

        // Clic gauche -> tentative mulligan
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, cardLayerMask))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log($"[CardClickHandler] {cardView.Card.Data.name} clicked for mulligan.");
                    OnCardClicked?.Invoke(cardView.Card);
                    MulliganActive = false;
                }
            }
        }
    }

    void OnDisable()
    {
        if (highlighted)
            Highlight(false);
    }

    private void Highlight(bool enable)
    {
        // TODO : VFX (outline, glow...)
    }
}
