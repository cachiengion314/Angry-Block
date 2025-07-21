using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  readonly Dictionary<int, int> _firingPositionIndexes = new();
  [SerializeField] Transform _firingPositions;
  readonly List<GameObject> _firingSlots = new();
  [Range(1f, 50f)]
  [SerializeField] float rotationSpeed = 5f;
  [Range(1f, 200)]
  [SerializeField] float bulletSpeed = 10.0f;
  [Range(1f, 30)]
  [SerializeField] float wanderingSpeed = 1.0f;

  void WanderingAroundUpdate(GameObject blastBlock)
  {
    if (!_firingPositionIndexes.ContainsKey(blastBlock.GetInstanceID()))
      _firingPositionIndexes.Add(blastBlock.GetInstanceID(), 0);
    var idx = _firingPositionIndexes[blastBlock.GetInstanceID()];
    var currentIdx = idx % _firingPositions.childCount;
    var startPos = _firingPositions.GetChild(currentIdx).position;
    var targetIdx = (idx + 1) % _firingPositions.childCount;
    var targetPos = _firingPositions.GetChild(targetIdx).position;

    HoangNam.Utility.InterpolateMoveUpdate(
      blastBlock.transform.position,
      startPos,
      targetPos,
      updateSpeed * wanderingSpeed,
      out var t,
      out var nextPos
    );
    blastBlock.transform.position = nextPos;
    if (t < 1) return;

    _firingPositionIndexes[blastBlock.GetInstanceID()] = idx + 1;
  }

  GameObject ChooseTargetFrom(List<GameObject> colorBlocks, GameObject blastBlock)
  {
    for (int i = 0; i < colorBlocks.Count; ++i)
    {
      var obj = colorBlocks[i];
      if (obj == null) continue;
      if (!obj.TryGetComponent<IDamageable>(out var damageable)) continue;
      if (damageable.GetWhoPicked() == blastBlock) return obj;
      if (damageable.GetWhoPicked() == null) return obj;
    }
    return null;
  }

  void LockAndFireTargetUpddate()
  {
    for (int i = _firingSlots.Count - 1; i >= 0; --i)
    {
      if (_firingSlots[i] == null) continue;

      var blastBlock = _firingSlots[i];
      if (!blastBlock.TryGetComponent<IMoveable>(out var moveable)) continue;
      if (!moveable.GetLockedPosition().Equals(0)) continue;
      if (!blastBlock.TryGetComponent<IGun>(out var gun)) continue;
      if (gun.GetAmmunition() <= 0)
      {
        // var arr = new Vector3[2] { new(10, 0, 0), new(-10, 0, 0) };
        // var targetPos = arr[UnityEngine.Random.Range(0, arr.Length)];
        // blastBlock.transform.DOMove(targetPos, .7f).OnComplete(() => Destroy(blastBlock));
        Destroy(blastBlock);

        _firingSlots.Remove(blastBlock);
        continue;
      }

      WanderingAroundUpdate(blastBlock);

      if (!blastBlock.TryGetComponent<IColorBlock>(out var dirColor)) continue;
      var colorBlocks = FindColorBlocksMatchedFor(blastBlock);
      if (colorBlocks.Count == 0)
      {
        // cannot find any targets
        continue;
      }
      var target = ChooseTargetFrom(colorBlocks, blastBlock);
      if (target == null) continue;
      if (!target.TryGetComponent<IDamageable>(out var damageable)) continue;
      if (damageable.GetWhoPicked() != null && damageable.GetWhoPicked() != blastBlock)
        continue;

      damageable.SetWhoPicked(blastBlock); // picking this target to prevent other interfere
      var dirToTarget = target.transform.position - blastBlock.transform.position;
      var targetRad = math.acos(
        math.dot(dirToTarget.normalized, blastBlock.transform.up)
      );
      if (math.abs(targetRad) > .1f)
      {
        var sign = math.sign(
          math.cross(blastBlock.transform.up, dirToTarget).z
        );
        var deltaTargetRad = updateSpeed * sign * rotationSpeed * Time.deltaTime * targetRad;
        var deltaQuad = new Quaternion(
          0, 0, math.sin(deltaTargetRad / 2f), math.cos(deltaTargetRad / 2f)
        );
        blastBlock.transform.rotation *= deltaQuad;
        continue;
      }

      damageable.SetWhoPicked(null);
      if (
       _firingPositionIndexes.ContainsKey(blastBlock.GetInstanceID())
        && _firingPositionIndexes[blastBlock.GetInstanceID()] % _firingPositions.childCount != 0
      ) continue; // if block is not standing at firing zone its should not permitted for firing

      // standing and firing at target
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
      if (_waitingTimers.ContainsKey(blastBlock.GetInstanceID()))
        _waitingTimers[blastBlock.GetInstanceID()] = 0f;
    }
  }
}