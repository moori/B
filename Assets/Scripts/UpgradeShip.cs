using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

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
    public TextMeshPro text;
    public float hidePositionY = 14f;
    public float showPositionY = 0f;

    public enum UpgradeType
    {
        None,
        Firerate,
        MaxHP,
        ShieldSlot,
        Missile,
        Heal
    }
    private void Awake()
    {
        transform.DOLocalMoveY(hidePositionY, 0f);
    }

    public void Show()
    {
        var delay = upgradeType == UpgradeType.None ? 4f : 0f;
        gameObject.SetActive(true);
        transform.DOLocalMoveY(showPositionY, 2f).SetEase(Ease.InOutQuad);
        fill.transform.localScale = Vector3.zero;
        isActive = true;
    }
    public void Hide()
    {
        StopAllCoroutines();
        transform.DOLocalMoveY(hidePositionY, 2f).SetEase(Ease.InOutQuad).OnComplete(()=> {
            gameObject.SetActive(false);
        });
    }

    public void SetShip(UpgradeType type)
    {
        upgradeType = type;
        switch (type)
        {
            case UpgradeType.None:
                text.text = "dismiss";
                break;
            case UpgradeType.Firerate:
                text.text = "+fire rate";
                break;
            case UpgradeType.MaxHP:
                text.text = "+max hp";
                break;
            case UpgradeType.ShieldSlot:
                text.text = "+shield slot";
                break;
            case UpgradeType.Missile:
                text.text = "+missile";
                break;
            case UpgradeType.Heal:
                text.text = "heal";
                break;
            default:
                break;
        }
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
                modelTransform.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.InQuad).OnComplete(()=> {
                    hitsBuffer[0].GetComponent<Player>().ApplyUpgrade(upgradeType);
                    OnUpgrade?.Invoke();
                });
            }
        }
        else
        {
            isConfirming = false;
            fill.transform.localScale = Vector3.zero;
            modelTransform.rotation = Quaternion.identity;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(detectionOffset), detectionRadius);
    }
}
