public interface IDamageable
{
  public void SetInitHealth(int health);
  public int GetHealth();
  public void SetHealth(int health);
  public bool IsDead();
  public bool IsDamage();
}