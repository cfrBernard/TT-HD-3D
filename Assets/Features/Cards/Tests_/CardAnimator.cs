using DG.Tweening;
using UnityEngine;

public class CardAnimator : MonoBehaviour
{
    public float moveDuration = 0.5f;
    public float flipDuration = 0.3f;

    public void AnimateTo(Vector3 targetPos, Quaternion targetRot, bool doFlip, float delay = 0f)
    {
        transform.DOKill();
    
        // Déplacement avec délai
        transform.DOLocalMove(targetPos, moveDuration).SetEase(Ease.OutCubic).SetDelay(delay);
    
        if (doFlip)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOLocalRotate(new Vector3(0, 180f, 0), flipDuration).SetEase(Ease.InQuad).SetDelay(delay));
            seq.Append(transform.DOLocalRotate(new Vector3(0, 0, 0), flipDuration).SetEase(Ease.OutQuad));
        }
        else
        {
            // rotation finale après le délai
            DOVirtual.DelayedCall(delay, () => transform.localRotation = targetRot);
        }
    }

}
