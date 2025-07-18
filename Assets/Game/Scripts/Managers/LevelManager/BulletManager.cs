using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public partial class LevelManager : MonoBehaviour
{
  [Header("Bullet Manager")]
  ObjectPool<BulletControl> _bulletsPool;
  public ObjectPool<BulletControl> BulletsPool { get { return _bulletsPool; } }
  List<BulletControl> _activeBullets;
  readonly float KEY_BULLET_LIFE_DURATION = 1.9f;

  void InitPool()
  {
    _bulletsPool = new ObjectPool<BulletControl>(
      CreateBulletsPool,
      OnTake,
      OnRelease,
      OnDispose,
      true,
      100,
      100
    );
    _activeBullets = new List<BulletControl>();
  }

  BulletControl CreateBulletsPool()
  {
    BulletControl obj = Instantiate(bulletPref, transform.position, bulletPref.transform.rotation);
    return obj;
  }

  void OnTake(BulletControl obj)
  {
    obj.gameObject.SetActive(true);
    if (obj.TryGetComponent<IBullet>(out var bullet))
      bullet.SetLifeTimer(0);
    _activeBullets.Add(obj);
  }

  void OnRelease(BulletControl obj)
  {
    obj.gameObject.SetActive(false);
    _activeBullets.Remove(obj);
  }

  void OnDispose(BulletControl obj)
  {
    Destroy(obj);
  }

  void CleanReleaseFor(BulletControl bullet)
  {
    if (!bullet.TryGetComponent<IMoveable>(out var bullMoveable)) return;
    var lockedTarget = bullMoveable.GetLockedTarget();
    if (
      lockedTarget != null
      && lockedTarget.TryGetComponent<IDamageable>(out var lockedDamageable)
    )
    {
      lockedDamageable.SetWhoLocked(null);
    }
    if (bullet.gameObject.activeSelf)
    {
      _bulletsPool.Release(bullet);
    }
  }

  void BulletPositionsUpdate()
  {
    for (int i = 0; i < _activeBullets.Count; ++i)
    {
      var bullet = _activeBullets[i];
      if (!bullet.TryGetComponent<IMoveable>(out var bullMoveable)) continue;
      if (!bullet.TryGetComponent<IBullet>(out var bulletComp)) continue;
      if (bulletComp.GetLifeTimer() > KEY_BULLET_LIFE_DURATION)
      {
        CleanReleaseFor(bullet);
        continue;
      }
      bulletComp.SetLifeTimer(
        bulletComp.GetLifeTimer() + Time.deltaTime
      );
      InterpolateMoveUpdate(
        bullet.transform.position,
        bullMoveable.GetInitPostion(),
        bullMoveable.GetLockedPosition(),
        updateSpeed * bulletSpeed,
        out var t,
        out var nextPos
      );
      bullet.transform.position = nextPos;
      if (t < 1) continue;

      var targetBlock = bullMoveable.GetLockedTarget();
      if (!targetBlock.TryGetComponent<IDamageable>(out var colorBlockDamageable)) continue;

      colorBlockDamageable.SetHealth(colorBlockDamageable.GetHealth() - bullet.GetDamage());
      colorBlockDamageable.SetWhoLocked(null);
      if (!colorBlockDamageable.IsDead()) continue;

      if (!targetBlock.TryGetComponent<IColorBlock>(out var colorBlock)) continue;

      _colorBlocks[colorBlock.GetIndex()] = null;
      Destroy(targetBlock.gameObject);

      CleanReleaseFor(bullet);
    }
  }
}