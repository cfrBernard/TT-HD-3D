using UnityEngine;

[RequireComponent(typeof(CardAnimator))]
public class CardView : MonoBehaviour
{
    public Card Card { get; private set; }
    public MeshRenderer renderer;
    private MaterialPropertyBlock mpb;
    private CardAnimator animator;

    [Header("Owner visuals")]
    public CardOwnerView ownerView;
    public GameObject hoverFX;

    private void Awake()
    {
        animator = GetComponent<CardAnimator>();
    }

    public void Setup(Card card)
    {
        Card = card;
        Debug.Log($"[CardView] Setup for card '{card.Data.name}' (Owner: {card.Owner}).");

        if (mpb == null) mpb = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_FrontAlbedo", card.Data.frontAlbedo);
        renderer.SetPropertyBlock(mpb);

        ownerView?.Setup(card);
    }

    public void AnimateDraw(Vector3 fromPos, Transform targetSlot, bool doFlip, float delay)
    {
        animator.AnimateDraw(fromPos, targetSlot, doFlip, delay);
    }

    public void UpdateOwnerVisual()
    {
        ownerView?.UpdateOwnerVisual();
    }

    public void EnableHoverFX()
    {
        hoverFX.SetActive(true);
    }

    public void DisableHoverFX()
    {
         hoverFX.SetActive(false);
    }
}
