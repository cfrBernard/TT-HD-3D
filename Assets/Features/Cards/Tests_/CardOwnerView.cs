using UnityEngine;

public class CardOwnerView : MonoBehaviour
{
    [Header("Refs")]
    public MeshRenderer ownerFlagRenderer;
    public ParticleSystem vfxPlayer1;
    public ParticleSystem vfxPlayer2;

    [Header("Materials")]
    public Material matPlayer1;
    public Material matPlayer2;

    private Card card;

    public void Setup(Card card)
    {
        this.card = card;
        UpdateOwnerVisual(); // setup initial
    }

    public void UpdateOwnerVisual()
    {
        if (card == null || card.Owner == null) return;

        if (card.Owner.Name == "Player1")
        {
            ownerFlagRenderer.material = matPlayer1;
            if (vfxPlayer1 != null) vfxPlayer1.Play();
        }
        else
        {
            ownerFlagRenderer.material = matPlayer2;
            if (vfxPlayer2 != null) vfxPlayer2.Play();
        }
    }
}
