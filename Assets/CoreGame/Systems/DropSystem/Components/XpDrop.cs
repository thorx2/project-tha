using System.Collections;
using DG.Tweening;
using Lean.Pool;
using SuperMaxim.Messaging;
using UnityEngine;

public class XpDrop : MonoBehaviour
{
    [SerializeField]
    private int XpOnCollect;

    [SerializeField]
    private AudioClip effectClip;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        AudioManager.Instance.PlaySFX(effectClip);
        transform.DOMove(collision.transform.position, 0.25f).SetEase(Ease.InCirc).OnComplete(() =>
        {
            Messenger.Default.Publish(new OnXpCollect());
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(FrameSkipDespawn());
            }
        });
    }

    private IEnumerator FrameSkipDespawn()
    {
        yield return new WaitForEndOfFrame();
        LeanPool.Despawn(gameObject);
    }
}
