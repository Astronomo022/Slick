using UnityEngine;

public class EntStats : MonoBehaviour
{
    public int damage, maxHP, curHP;

    public bool Damage(int dmg)
    {
        curHP -= dmg;
        if (curHP <= 0)
        {
            curHP = 0;
            return true; // Entity is dead
        }
        return false; // Entity is still alive
    }

    public void Heal(int regenRate)
    {

        curHP += regenRate;
        if (curHP > maxHP)
        {
            curHP = maxHP;
            
        }
        
    }
}
