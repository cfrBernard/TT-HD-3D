using UnityEngine;

[RequireComponent(typeof(CardAnimator))]
public class CardView : MonoBehaviour
{
    public Card Card { get; private set; }
    public MeshRenderer renderer;
    private MaterialPropertyBlock mpb;
    private CardAnimator animator;

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
    }

    public void AnimateDraw(Vector3 fromPos, Transform targetSlot, bool doFlip, float delay)
    {
        animator.AnimateDraw(fromPos, targetSlot, doFlip, delay);
    }
}
