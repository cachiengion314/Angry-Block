using Unity.Mathematics;
using UnityEngine;

public class BallControl : MonoBehaviour
{
  [SerializeField] Rigidbody2D rb2D;
  [SerializeField] GameObject Train;
  [SerializeField] float force;
  public float3 spawnBallPos;
  public int mass;

  public void InitBall()
  {
    transform.localPosition = spawnBallPos;
    rb2D.simulated = false;
    rb2D.linearVelocity = Vector2.zero;
    Train.SetActive(false);
  }
  public void DropBall()
  {
    rb2D.simulated = true;
    Train.SetActive(true);
  }
  void AddForce()
  {
    int randomIndex = UnityEngine.Random.Range(0, 99);
    if (randomIndex < 50)
    {
      rb2D.linearVelocityX = force;
    }
    else
    {
      rb2D.linearVelocityX = -force;
    }
    rb2D.linearVelocityY += force;
  }
  DotControl currentDot;
  void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject.TryGetComponent(out DotControl dot))
    {
      if (currentDot == dot) return;
      SoundManager.Instance.PlayDinhDinhSfx();
      currentDot = dot;
      dot.OnColectionEnter();
      AddForce();
    }
    if (collision.gameObject.TryGetComponent(out RewardControl reward))
    {
      SoundManager.Instance.PlayGetRewardSfx();
      reward.GetReward(mass);
      reward.OnColectionEnter();
      InitBall();
    }
  }
}
