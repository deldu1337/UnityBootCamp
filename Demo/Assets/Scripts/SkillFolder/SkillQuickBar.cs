using UnityEngine;

public class SkillQuickBar : MonoBehaviour
{
    // A,S,D,F,G,Z,X,C,V 슬롯 순서로 배치했다고 가정 (총 9개)
    public SkillSlotUI[] slots; // 각 슬롯에는 이미지(아이콘)가 있고, 이 스크립트를 붙여둠

    // 슬롯에 스킬 할당
    public void Assign(int index, string skillId, Sprite icon)
    {
        if (index < 0 || index >= slots.Length) return;
        slots[index].SetSkill(skillId, icon);
    }

    // 첫 빈 슬롯에 자동 할당 (없으면 false)
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

    // 스왑
    public void Swap(int a, int b)
    {
        if (a == b) return;
        if (a < 0 || b < 0 || a >= slots.Length || b >= slots.Length) return;
        var tmp = slots[a].GetData();
        slots[a].ApplyData(slots[b].GetData());
        slots[b].ApplyData(tmp);
    }

    // 외부에서 시전 요청 (키 입력)
    public string GetSkillAt(int index)
    {
        if (index < 0 || index >= slots.Length) return null;
        return slots[index].SkillId;
    }
}
