using UnityEngine;

public class SmokeTrail : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;

    public void ChangeColorTo(Color color)
    {
        var rend = _particleSystem.GetComponent<ParticleSystemRenderer>();
        rend.material.SetColor("_Color", color);
    }
}
