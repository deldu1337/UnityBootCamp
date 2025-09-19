using UnityEngine;

public class SkillQuickBar : MonoBehaviour
{
    // A,S,D,F,G,Z,X,C,V ���� ������ ��ġ�ߴٰ� ���� (�� 9��)
    public SkillSlotUI[] slots; // �� ���Կ��� �̹���(������)�� �ְ�, �� ��ũ��Ʈ�� �ٿ���

    // ���Կ� ��ų �Ҵ�
    public void Assign(int index, string skillId, Sprite icon)
    {
        if (index < 0 || index >= slots.Length) return;
        slots[index].SetSkill(skillId, icon);
    }

    // ù �� ���Կ� �ڵ� �Ҵ� (������ false)
    public bool AssignToFirstEmpty(string skillId, Sprite icon)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (string.IsNullOrEmpty(slots[i].SkillId))
            {
                Assign(i, skillId, icon);
                return true;
            }
        }
        return false;
    }

    // ����
    public void Swap(int a, int b)
    {
        if (a == b) return;
        if (a < 0 || b < 0 || a >= slots.Length || b >= slots.Length) return;
        var tmp = slots[a].GetData();
        slots[a].ApplyData(slots[b].GetData());
        slots[b].ApplyData(tmp);
    }

    // �ܺο��� ���� ��û (Ű �Է�)
    public string GetSkillAt(int index)
    {
        if (index < 0 || index >= slots.Length) return null;
        return slots[index].SkillId;
    }
}
