using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LaserWeapon : MonoBehaviour
{
    public SpriteRenderer aimSprite;
    public SpriteRenderer fireSprite;
    public ParticleSystem chargePart;
    public ParticleSystem firePart;

    public FacePlayer facePlayerComponent;

    [Header("Attack")]
    public float delayBetweenAttacks;
    public float aimdDuration;
    public float stillBeforeShotDuration;
    public float laserDuration;
    public LayerMask targetLayerMask;
    public Vector2 laserCenter;
    public Vector2 laserSize;
    public int damage;

    Collider2D[] hitsBuffer = new Collider2D[1];
    private bool isFiring;

    private void OnEnable()
    {
        StartCoroutine(AttackRoutine());
        GameController.OnGameOver += Stop;
        aimSprite.gameObject.SetActive(false);
        fireSprite.transform.DOScaleX(0, 0f);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isFiring = false;
        fireSprite.transform.DOScaleX(0f, .15f);
        facePlayerComponent.enabled = true;
        firePart.gameObject.SetActive(false);
        GameController.OnGameOver -= Stop;
    }

    public void Stop()
    {
        StopAllCoroutines(); 
        isFiring = false;
        fireSprite.transform.DOScaleX(0f, .15f);
        facePlayerComponent.enabled = true;
        firePart.gameObject.SetActive(false);
        chargePart.gameObject.SetActive(false);
        aimSprite.gameObject.SetActive(false);
        GameController.OnGameOver -= Stop;
    }

    public IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (!GameController.IsGameOver)
        {
            aimSprite.gameObject.SetActive(true);
            yield return new WaitForSeconds(aimdDuration);


            facePlayerComponent.enabled = false;
            chargePart.gameObject.SetActive(true);
            aimSprite.transform.localScale = new Vector3(0.06f, aimSprite.transform.localScale.y, aimSprite.transform.localScale.z); 
            yield return new WaitForSeconds(stillBeforeShotDuration);


            isFiring = true;
            aimSprite.gameObject.SetActive(false);
            fireSprite.transform.DOScaleX(laserSize.x, .15f);
            firePart.gameObject.SetActive(true);
            chargePart.gameObject.SetActive(false);
            yield return new WaitForSeconds(laserDuration);

            isFiring = false;
            fireSprite.transform.DOScaleX(0f, .15f);
            facePlayerComponent.enabled = true;
            firePart.gameObject.SetActive(false);
            aimSprite.transform.localScale = new Vector3(0.03f, aimSprite.transform.localScale.y, aimSprite.transform.localScale.z);
            yield return new WaitForSeconds(delayBetweenAttacks);
        }
    }

    private void FixedUpdate()
    {
        if (!isFiring) return;

        var numHits = Physics2D.OverlapBoxNonAlloc(transform.TransformPoint(laserCenter), laserSize, transform.eulerAngles.z , hitsBuffer, targetLayerMask.value);

        for (int i = 0; i < numHits; i++)
        {
            var hitTarget = hitsBuffer[i].gameObject.GetComponent<HitTarget>();
            hitTarget.Hit(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.TransformPoint(laserCenter), laserSize);
    }
}
