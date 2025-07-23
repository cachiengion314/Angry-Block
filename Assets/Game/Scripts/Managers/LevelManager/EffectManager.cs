using Unity.Cinemachine;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  [SerializeField] CinemachineImpulseSource impulseSource;

  void ShakeCameraBy(Vector3 force)
  {
    impulseSource.GenerateImpulse(force);
  }
}