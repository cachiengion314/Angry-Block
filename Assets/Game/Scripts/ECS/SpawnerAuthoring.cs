using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
  public GameObject prefab;
  public int count;
  public Vector3 offset;
}

public struct Spawner : IComponentData
{
  public Entity Prefab;
  public int Count;
  public float3 Offset;
}

public class SpawnerBaker : Baker<SpawnerAuthoring>
{
  public override void Bake(SpawnerAuthoring authoring)
  {
    var entity = GetEntity(TransformUsageFlags.Dynamic);
    AddComponent(entity, new Spawner
    {
      Prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
      Count = authoring.count,
      Offset = authoring.offset
    });
  }
}
