using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class RewardControl : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprRenderer;
    float lever;
    public void SetLever(float lever, Sprite sprite)
    {
        this.lever = lever;
        sprRenderer.sprite = sprite;
    }
    public void GetReward(int mass)
    {
        int coin = (int)(mass * lever);
        sprRenderer.material.SetFloat("_brightness", 2f);
        EffectManager.Instance.EmissingCoinsWithParticleTo(
        SpawnPlinko.Instance.coinPos,
        10, sprRenderer.transform,
        () =>
        {
            sprRenderer.material.SetFloat("_brightness", 1f);
            GameManager.Instance.CurrentCoin += coin;
            SpawnPlinko.Instance.OnButton();
        }
        );
    }

    public void OnColectionEnter()
    {
        sprRenderer.transform.DOKill();
        sprRenderer.transform.localPosition = float3.zero;
        sprRenderer.transform
        .DOShakePosition(0.3f, new Vector3(0f, -0.6f, 0f), 1, 90f, false, true);
    }
}
