using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  [Header("Prefs")]
  [SerializeField] BulletControl bulletPref;
  [SerializeField] ColorBlockControl colorBlockPref;
  [SerializeField] DirectionBlockControl directionBlockPref;
  [SerializeField] WoodenBlockControl woodenBlockControlPref;
  [SerializeField] IceBlockControl iceBlockControlPref;
  [SerializeField] BlastBlockControl blastBlockPref;
  [SerializeField] TunnelControl tunnelControlPref;

  public ColorBlockControl SpawnColorBlockAt(float3 pos)
  {
    var obj = Instantiate(colorBlockPref, pos, colorBlockPref.transform.rotation);
    return obj;
  }

  public ColorBlockControl SpawnColorBlockAt(int index, Transform parent)
  {
    var pos = topGrid.ConvertIndexToWorldPos(index);
    var obj = Instantiate(colorBlockPref, parent);
    obj.transform.position = pos;
    return obj;
  }

  public DirectionBlockControl SpawnDirectionBlockAt(int index, Transform parent)
  {
    var pos = bottomGrid.ConvertIndexToWorldPos(index);
    var obj = Instantiate(directionBlockPref, parent);
    obj.transform.position = pos;
    return obj;
  }

  public WoodenBlockControl SpawnWoondenBlockAt(int index, Transform parent)
  {
    var pos = bottomGrid.ConvertIndexToWorldPos(index);
    var obj = Instantiate(woodenBlockControlPref, parent);
    obj.transform.position = pos;
    return obj;
  }

  public IceBlockControl SpawnIceBlockAt(int index, Transform parent)
  {
    var pos = bottomGrid.ConvertIndexToWorldPos(index);
    var obj = Instantiate(iceBlockControlPref, parent);
    obj.transform.position = pos;
    return obj;
  }

  public BulletControl SpawnBulletAt(float3 pos, float3 velocity, int _damage = 1)
  {
    var bullet = _bulletsPool.Get();
    bullet.transform.position = pos;
    bullet.SetVelocity(velocity);
    bullet.SetDamage(_damage);
    return bullet;
  }

  public BlastBlockControl SpawnBlastBlockAt(float3 pos, Transform parent)
  {
    var obj = Instantiate(blastBlockPref, parent);
    obj.transform.position = pos;
    return obj;
  }

  public TunnelControl SpawnTunnelAt(int index, Transform parent)
  {
    var pos = bottomGrid.ConvertIndexToWorldPos(index);
    var obj = Instantiate(tunnelControlPref, parent);
    obj.transform.position = pos;
    return obj;
  }
}
