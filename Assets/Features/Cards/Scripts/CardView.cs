using UnityEngine;

public class CardView : MonoBehaviour
{
    public MeshRenderer renderer;
    private MaterialPropertyBlock mpb;

    public void Setup(CardData data)
    {
        if (mpb == null) mpb = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_FrontAlbedo", data.frontAlbedo);
        renderer.SetPropertyBlock(mpb);
    }
}