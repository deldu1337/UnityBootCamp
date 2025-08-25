using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int hp = 100;
    
    public void SetHP(int hp)
        { this.hp = hp; }
    public int GetHP()
        { return hp; }
}
