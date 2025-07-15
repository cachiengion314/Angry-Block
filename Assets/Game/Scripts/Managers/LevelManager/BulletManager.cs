using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public partial class LevelManager : MonoBehaviour
{
  [Header("Bullet Manager")]
  ObjectPool<BulletControl> _bulletsPool;
  public ObjectPool<BulletControl> BulletsPool { get { return _bulletsPool; } }
  List<BulletControl> _activeBullets;

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

  void BulletPositionsUpdate()
  {
    for (int i = 0; i < _activeBullets.Count; ++i)
    {
      var bullet = _activeBullets[i];
      bullet.transform.position += Time.deltaTime * (Vector3)bullet.GetVelocity();

      var currentPos = bullet.transform.position;
      if (topGrid.IsPosOutsideAt(currentPos)) continue;

      var idx = topGrid.ConvertWorldPosToIndex(currentPos);
      if (_colorBlocks[idx] == null) continue;

      var colorBlock = _colorBlocks[idx];
      if (!colorBlock.TryGetComponent<IDamageable>(out var damageable)) continue;
      if (damageable.IsDead()) continue;

      damageable.SetHealth(damageable.GetHealth() - bullet.GetDamage());
      if (!damageable.IsDead()) continue;

      _colorBlocks[colorBlock.GetIndex()] = null;
      Destroy(colorBlock.gameObject);

      if (bullet.gameObject.activeSelf)
      {
        _bulletsPool.Release(bullet);
      }
    }
  }
}