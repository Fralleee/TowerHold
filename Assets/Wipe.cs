using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Wipe")]
public class Wipe : Spell
{
  public override void Perform()
  {
    base.Perform();

    foreach (var enemy in Enemy.AllEnemies.ToArray())
    {
      enemy.TakeDamage(enemy.Health);
    }
  }
}