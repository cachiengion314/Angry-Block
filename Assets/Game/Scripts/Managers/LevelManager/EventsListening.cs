using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  void OnOutOfAmmunition(GameObject blastBlock)
  {
    if (!blastBlock.TryGetComponent<ISpriteRend>(out var sprite)) return;
    if (DOTween.IsTweening(sprite.GetBodyRenderer().transform)) return;

    var duration = .2f;
    sprite.GetBodyRenderer().DOColor(Color.gray, duration * 2f);
    var originalScale = sprite.GetBodyRenderer().transform.localScale;
    sprite.GetBodyRenderer().transform.DOScale(1.3f * originalScale, duration)
      .SetLoops(2, LoopType.Yoyo)
      .OnComplete(
        () =>
        {
          sprite.GetBodyRenderer().transform.DOScale(.7f * originalScale, duration)
          .SetLoops(2, LoopType.Incremental)
          .OnComplete(
            () =>
            {
              _firingSlots.Remove(blastBlock);
              SpawnColorSplashEfxAt(blastBlock.transform.position);
              ShakeCameraBy(new float3(.0f, -.25f, .0f));
              Destroy(blastBlock);
            }
          );
        }
      );
  }

  void OnFireTarget(GameObject blastBlock, GameObject target)
  {
    if (!target.TryGetComponent<IDamageable>(out var damageable)) return;

    blastBlock.GetComponent<IGun>().SetAmmunition(
      blastBlock.GetComponent<IGun>().GetAmmunition() - 1
    );
    var bullet = SpawnBulletAt(
      blastBlock.transform.position,
      updateSpeed * bulletSpeed,
      1
    );
    damageable.SetWhoLocked(bullet.gameObject);
    if (bullet.TryGetComponent<IBullet>(out var bulletComp))
    {
      bulletComp.SetLifeTimer(0);
    }
    if (bullet.TryGetComponent<IMoveable>(out var moveableBullet))
    {
      moveableBullet.SetInitPostion(bullet.transform.position);
      moveableBullet.SetLockedPosition(target.transform.position);
      moveableBullet.SetLockedTarget(target.transform);
    }

    blastBlock.transform
      .DOScale(.9f, fireRate / 2f)
      .SetLoops(2, LoopType.Yoyo);

    if (!blastBlock.TryGetComponent<ISpriteRend>(out var sprite)) return;
    var dirToTarget = (target.transform.position - blastBlock.transform.position).normalized;
    var targetPos = Vector3.zero + -1 * .2f * dirToTarget;
    sprite.GetBodyRenderer()
      .transform.DOLocalMove(targetPos, fireRate / 2f)
      .SetLoops(2, LoopType.Yoyo);
  }

  void OnColorBlockDestroyedByBullet(GameObject colorBlock)
  {
    var duration = .12f;
    colorBlock.transform
     .DOScale(1.3f, duration)
     .OnComplete(() =>
     {
       SpawnColorSplashEfxAt(colorBlock.transform.position);
       Destroy(colorBlock);
     });

    if (!colorBlock.TryGetComponent<ISpriteRend>(out var sprite)) return;
    sprite.SetSortingOrder(sprite.GetSortingOrder() + 1);
    sprite.GetBodyRenderer()
     .DOColor(Color.yellow, duration);
    var targetPos = colorBlock.transform.position + Vector3.up * .3f;
    sprite.GetBodyRenderer()
      .transform.DOMove(targetPos, duration);
  }

  void OnMergedCollided(GameObject blast)
  {
    SpawnColorSplashEfxAt(blast.transform.position);
    ShakeCameraBy(new float3(.0f, .25f, .0f));

    if (!blast.TryGetComponent<ISpriteRend>(out var blastSprite)) return;
    if (!blast.TryGetComponent<IColorBlock>(out var blastColor)) return;

    blastSprite.GetBodyRenderer().color = Color.yellow;
    var originalColor = RendererSystem.Instance.GetColorBy(blastColor.GetColorValue());
    blastSprite.GetBodyRenderer().DOColor(originalColor, .5f);
  }
}
