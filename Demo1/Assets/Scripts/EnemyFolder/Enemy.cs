using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int hp = 100;  // ���� ü��, �ܺο��� ���� ���� �Ұ�

    // �� ü�� ����
    public void SetHP(int hp)
    {
        this.hp = hp; // ���޹��� ������ ü�� ����
    }

    // �� ü�� ��ȸ
    public int GetHP()
    {
        return hp; // ���� ü�� ��ȯ
    }
}