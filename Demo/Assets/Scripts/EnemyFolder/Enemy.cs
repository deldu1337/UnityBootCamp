using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int hp = 100;  // 적의 체력, 외부에서 직접 접근 불가

    // 적 체력 설정
    public void SetHP(int hp)
    {
        this.hp = hp; // 전달받은 값으로 체력 설정
    }

    // 적 체력 조회
    public int GetHP()
    {
        return hp; // 현재 체력 반환
    }
}