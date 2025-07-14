public interface IDamageable
{
  public void SetInitHealth(int health);
  public int GetHealth();
  public void SetHealth(int health);
  public bool IsDead();
  public bool IsDamage();
  public DirectionBlockControl GetWhoLocked();
  public DirectionBlockControl GetWhoPicked();
  public void SetWhoLocked(DirectionBlockControl block);
  public void SetWhoPicked(DirectionBlockControl block);
}