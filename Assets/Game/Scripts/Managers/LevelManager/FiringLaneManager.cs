using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  readonly Dictionary<int, int> _firingPositionIndexes = new();
  [SerializeField] Transform _firingPositions;
  readonly List<GameObject> _firingSlots = new();
  [Range(1f, 10f)]
  [SerializeField] float rotationSpeed = 3.5f;
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

    InterpolateMoveUpdate(
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

  void LockAndFireTargetUpddate()
  {
    for (int i = 0; i < _firingSlots.Count; ++i)
    {
      if (_firingSlots[i] == null) continue;

      var blastBlock = _firingSlots[i];
      if (!blastBlock.TryGetComponent<IMoveable>(out var moveable)) continue;
      if (!moveable.GetLockedPosition().Equals(0)) continue;
      if (!blastBlock.TryGetComponent<IGun>(out var gun)) continue;
      if (gun.GetAmmunition() <= 0)
      {
        var idx = FindSlotFor(blastBlock, _firingSlots);
        if (idx < 0 || idx > _firingSlots.Count - 1) continue;

        _firingSlots[idx] = null;
        Destroy(blastBlock);
        continue;
      }

      WanderingAroundUpdate(blastBlock);

      if (!blastBlock.TryGetComponent<IColorBlock>(out var dirColor)) continue;
      var colorBlock = FindFirstBlockMatchedFor(blastBlock);
      if (colorBlock == null)
      {
        // cannot find any targets so this blastBlock should go to the waiting slot
        // ShouldGoWaitingUpdate(blastBlock);
        continue;
      }

      if (!colorBlock.TryGetComponent<IDamageable>(out var damageable)) continue;
      if (damageable.GetWhoPicked() != null && damageable.GetWhoPicked() != blastBlock)
        continue;

      damageable.SetWhoPicked(blastBlock); // picking this target to prevent other interfere
      var dirToTarget = colorBlock.transform.position - blastBlock.transform.position;
      var targetRad = math.acos(
        math.dot(dirToTarget.normalized, blastBlock.transform.up)
      );
      if (math.abs(targetRad) > .1f)
      {
        var sign = math.sign(
          math.cross(blastBlock.transform.up, dirToTarget).z
        );
        var deltaTargetRad = sign * rotationSpeed * Time.deltaTime * targetRad;
        var deltaQuad = new Quaternion(
          0, 0, math.sin(deltaTargetRad / 2f), math.cos(deltaTargetRad / 2f)
        );
        blastBlock.transform.rotation *= deltaQuad;
        continue;
      }
      damageable.SetWhoPicked(null);
      if (damageable.GetWhoLocked() == blastBlock) continue;
      if (
       _firingPositionIndexes.ContainsKey(blastBlock.GetInstanceID())
        && _firingPositionIndexes[blastBlock.GetInstanceID()] % _firingPositions.childCount != 0
      ) continue; // if block is not standing at firing zone its should not permitted for firing

      damageable.SetWhoLocked(blastBlock);
      // standing to fire to target
      blastBlock.GetComponent<IGun>().SetAmmunition(
        blastBlock.GetComponent<IGun>().GetAmmunition() - 1
      );
      var bullet = SpawnBulletAt(
        blastBlock.transform.position,
        updateSpeed * bulletSpeed * (
          colorBlock.transform.position - blastBlock.transform.position
        ).normalized,
        1
      );
      if (bullet.TryGetComponent<IBullet>(out var bulletComp))
      {
        bulletComp.SetLifeTimer(0);
      }
      if (bullet.TryGetComponent<IMoveable>(out var moveableBullet))
      {
        moveableBullet.SetInitPostion(bullet.transform.position);
        moveableBullet.SetLockedPosition(colorBlock.transform.position);
        moveableBullet.SetLockedTarget(colorBlock.transform);
      }
      if (_waitingTimers.ContainsKey(blastBlock.GetInstanceID()))
        _waitingTimers[blastBlock.GetInstanceID()] = 0f;
    }
  }
}