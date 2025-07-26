using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AngryBlock
{
  public partial struct SpawnSystem : ISystem
  {
    public void OnCreate(ref SystemState state)
    {
      state.RequireForUpdate<Spawner>();
      Debug.Log("Hello world! 1");
    }

    public void OnUpdate(ref SystemState state)
    {
      state.Enabled = false;
      var spawner = SystemAPI.GetSingleton<Spawner>();
      var spawnerEntity = SystemAPI.GetSingletonEntity<Spawner>();
      var spawnerTransform = SystemAPI.GetComponentRO<LocalTransform>(spawnerEntity);

      var instances = new NativeArray<Entity>(spawner.Count, Allocator.Temp);
      state.EntityManager.Instantiate(spawner.Prefab, instances);

      var offset = math.float3(spawnerTransform.ValueRO.Position);
      foreach (var entity in instances)
      {
        state.EntityManager.SetComponentData(entity, LocalTransform.FromPosition(offset));
        offset += spawner.Offset;
      }
    }
  }
}
