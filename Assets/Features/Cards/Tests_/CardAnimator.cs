using DG.Tweening;
using UnityEngine;

public class CardAnimator : MonoBehaviour
{
    public float moveDuration = 0.5f;
    public float flipDuration = 0.5f;

    public void AnimateDraw(Vector3 fromPos, Transform slot, bool doFlip, float delay)
    {
        Sequence seq = DOTween.Sequence();

        // Move en world depuis le deck vers le slot
        seq.Append(transform.DOMove(slot.position, moveDuration).SetEase(Ease.OutCubic).SetDelay(delay));
        seq.Join(transform.DORotateQuaternion(Quaternion.Euler(-90, 180, 0), moveDuration));

        // Parentage au slot + reset local
        seq.AppendCallback(() =>
        {
            transform.SetParent(slot, true); // conserve world position
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0, 180, 0); // back
        });

        // Flip progressif
        if (doFlip)
        {
            Sequence flipSeq = DOTween.Sequence();
            flipSeq.Append(transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 180, 0), flipDuration / 2f).SetEase(Ease.InQuad));
            flipSeq.Append(transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), flipDuration / 2f).SetEase(Ease.OutQuad));
            seq.Append(flipSeq);
        }
    }
}
