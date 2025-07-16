using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  [Range(1, 10)]
  [SerializeField] int firingSlotAmount = 4;
  float3[] _firingPositions;
  GameObject[] _firingSlots;
  [Range(1f, 10f)]
  [SerializeField] float rotationSpeed = 3.5f;
  [Range(1f, 30)]
  [SerializeField] float bulletSpeed = 10.0f;

  void InitFiringPositions()
  {
    _firingPositions = new float3[firingSlotAmount];
    _firingSlots = new GameObject[firingSlotAmount];

    var y = bottomGrid.GridSize.y - 1;
    var startX = 2;
    for (int x = startX; x < startX + firingSlotAmount; ++x)
    {
      if (x > bottomGrid.GridSize.x - 1) break;
      var pos = bottomGrid.ConvertGridPosToWorldPos(new int2(x, y));
      _firingPositions[x - startX] = pos;
    }
  }

  void LockAndFireTargetUpddate()
  {
    for (int i = 0; i < _firingSlots.Length; ++i)
    {
      if (_firingSlots[i] == null) continue;

      var directionBlock = _firingSlots[i];
      if (!directionBlock.TryGetComponent<IMoveable>(out var moveable)) continue;
      if (!moveable.GetLockedPosition().Equals(0)) continue;
      if (!directionBlock.TryGetComponent<IGun>(out var gun)) continue;
      if (gun.GetAmmunition() <= 0)
      {
        var idx = FindSlotFor(directionBlock, _firingSlots);
        if (idx < 0 || idx > _firingSlots.Length - 1) continue;

        _firingSlots[idx] = null;
        Destroy(directionBlock);
        continue;
      }

      if (!directionBlock.TryGetComponent<IColorBlock>(out var dirColor)) continue;
      var colorBlock = FindFirstBlockMatchedFor(directionBlock);
      if (colorBlock == null)
      {
        // cannot find any targets so this directionBlock should go to the waiting slot
        ShouldGoWaitingUpdate(directionBlock);
        continue;
      }

      if (!colorBlock.TryGetComponent<IDamageable>(out var damageable)) continue;
      if (damageable.GetWhoPicked() != null && damageable.GetWhoPicked() != directionBlock)
        continue;

      damageable.SetWhoPicked(directionBlock); // picking this target to prevent other interfere
      var dirToTarget = colorBlock.transform.position - directionBlock.transform.position;
      var targetRad = math.acos(
        math.dot(dirToTarget.normalized, directionBlock.transform.up)
      );
      if (math.abs(targetRad) > .1f)
      {
        var sign = math.sign(
          math.cross(directionBlock.transform.up, dirToTarget).z
        );
        var deltaTargetRad = sign * Time.deltaTime * rotationSpeed * targetRad;
        var deltaQuad = new Quaternion(
          0, 0, math.sin(deltaTargetRad / 2f), math.cos(deltaTargetRad / 2f)
        );
        directionBlock.transform.rotation *= deltaQuad;
        continue;
      }
      damageable.SetWhoPicked(null);
      if (damageable.GetWhoLocked() == directionBlock) continue;
      damageable.SetWhoLocked(directionBlock);
      // standing to fire to target
      directionBlock.GetComponent<IGun>().SetAmmunition(
        directionBlock.GetComponent<IGun>().GetAmmunition() - 1
      );
      var bullet = SpawnBulletAt(
        directionBlock.transform.position,
        updateSpeed * bulletSpeed * (
          colorBlock.transform.position - directionBlock.transform.position
        ).normalized,
        1
      );
      if (bullet.TryGetComponent<IMoveable>(out var moveableBullet))
      {
        moveableBullet.SetLockedPosition(colorBlock.transform.position);
        moveableBullet.SetLockedTarget(colorBlock.transform);
      }
      if (_waitingTimers.ContainsKey(directionBlock.GetInstanceID()))
        _waitingTimers[directionBlock.GetInstanceID()] = 0f;
    }
  }
}