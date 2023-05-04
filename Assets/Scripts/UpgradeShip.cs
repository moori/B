using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class UpgradeShip : MonoBehaviour
{

    public UpgradeType upgradeType;

    public LayerMask playerLayer;

    public float timeToConfirm;
    private float starTime;
    public float detectionRadius;
    public Vector3 detectionOffset;
    public SpriteRenderer fill;
    public Transform modelTransform;

    Collider2D[] hitsBuffer = new Collider2D[1];
    private bool isConfirming;
    public float rotationSpeed;
    private bool isActive;
    public Vector3 endLocalPosiion;

    public UnityEvent OnUpgrade;
    public ParticleSystem explosion;

    public enum UpgradeType
    {
        Firerate,
        MaxHP,
        ShieldSlot
    }
    private void Awake()
    {
        transform.DOLocalMoveY(14, 0f);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.DOLocalMoveY(0,2f).SetEase(Ease.InOutQuad);
        fill.transform.localScale = Vector3.zero;
        isActive = true;
    }
    public void Hide()
    {
        transform.DOLocalMoveY(14,2f).SetEase(Ease.InOutQuad).OnComplete(()=> {
            gameObject.SetActive(false);
        });
    }

    private void FixedUpdate()
    {
        if (!isActive) return;
        int numHits = Physics2D.OverlapCircleNonAlloc(transform.TransformPoint(detectionOffset), detectionRadius, hitsBuffer, playerLayer.value);

        if (numHits > 0)
        {
            if (!isConfirming)
            {
                isConfirming = true;
                starTime = Time.time;
            }

            var percent = (Time.time - starTime) / timeToConfirm;
            fill.transform.localScale = Vector3.one * percent;

            modelTransform.rotation *= Quaternion.Euler(0, rotationSpeed + (rotationSpeed * percent), 0);

            if (percent >= 1f)
            {
                isActive = false;
                explosion.gameObject.SetActive(true);
                modelTransform.DOLocalRotate(Vector3.zero, 1f).SetEase(Ease.InQuad).OnComplete(()=> {
                    hitsBuffer[0].GetComponent<Player>().ApplyUpgrade(upgradeType);
                    OnUpgrade?.Invoke();
                });
            }
        }
        else
        {
            isConfirming = false;
            fill.transform.localScale = Vector3.zero;
            modelTransform.DOLocalRotate(Vector3.zero, 1f).SetEase(Ease.InQuad);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(detectionOffset), detectionRadius);
    }
}
