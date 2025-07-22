using DG.Tweening;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  void OnOutOfAmmunition(GameObject blastBlock)
  {
    blastBlock.transform.DOScale(1.6f, .25f)
      .SetLoops(2, LoopType.Yoyo)
      .OnComplete(
        () =>
        {
          blastBlock.transform.DOScale(1.2f, .25f)
          .SetLoops(2, LoopType.Yoyo)
          .OnComplete(
            () =>
            {
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
      .DOScale(1.32f, fireRate / 2f)
      .SetLoops(2, LoopType.Yoyo);
  }
}
