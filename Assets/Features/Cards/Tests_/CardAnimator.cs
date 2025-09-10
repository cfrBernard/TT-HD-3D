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

    // Retourne la carte vers le deck, puis callback.
    public void AnimateReturn(Vector3 deckPos, System.Action onComplete)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMove(deckPos, moveDuration).SetEase(Ease.InBack));
        seq.Join(transform.DORotate(new Vector3(-90, 180, 0), flipDuration).SetEase(Ease.InCubic));

        seq.AppendCallback(() => onComplete?.Invoke());
    }

    // AIController PlaceCard
    public void AnimatePlay(Transform slot)
    {
        Sequence seq = DOTween.Sequence();

        // Déplacement vers le slot
        seq.Append(transform.DOMove(slot.position, moveDuration).SetEase(Ease.OutCubic));

        // Vérifie si la carte est à dos (y -180)
        bool isBack = Mathf.Approximately(transform.localRotation.eulerAngles.y, 180f);

        // Dans tous les cas, on recale le parent et la position -> !! besoin de faire ca sur le player aussi – donc besoin de refaire les rotate Slot/CardPrefab !!
        seq.AppendCallback(() =>
        {
            transform.SetParent(slot, true);
            transform.localPosition = Vector3.zero;
        });

        if (isBack)
        {
            // Ajoute le flip recto
            seq.Append(transform.DOLocalRotateQuaternion(Quaternion.Euler(-180, 0, 0), flipDuration).SetEase(Ease.OutCubic));
        }
        else
        {
            // Simple réorientation (au cas où)
            seq.Append(transform.DOLocalRotateQuaternion(Quaternion.Euler(-180, 0, 0), 0.2f));
        }
    }
}
