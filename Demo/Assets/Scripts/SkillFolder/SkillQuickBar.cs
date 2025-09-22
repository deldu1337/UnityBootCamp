//// SkillQuickBar.cs
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.UI;

//public class SkillQuickBar : MonoBehaviour
//{
//    public SkillSlotUI[] slots; // 에디터에서 비워놔도 AutoWireSlots로 채움

//    // ★ 자동 배선: 자식 구조에서 아이콘/쿨다운 찾아 등록 + 인덱스 부여
//    public void AutoWireSlots()
//    {
//        if (slots == null || slots.Length == 0)
//            slots = GetComponentsInChildren<SkillSlotUI>(true);

//        for (int i = 0; i < slots.Length; i++)
//        {
//            var s = slots[i];
//            if (!s) continue;

//            s.index = i;

//            if (!s.icon)
//            {
//                // 자식 중 첫 번째 '스킬 아이콘' 이미지 추적 (이름 A 라고 하셨으니 그쪽 먼저)
//                var iconTr = s.transform.Find("A");
//                if (!iconTr) iconTr = s.transform.GetComponentInChildren<Transform>(true);
//                var img = iconTr ? iconTr.GetComponent<Image>() : null;
//                if (img) s.icon = img;
//            }

//            if (!s.cooldownUI)
//            {
//                // MaskArea/CooldownOverlay 구조에서 Image 찾아 SkillCooldownUI 연결
//                var mask = s.transform.Find("MaskArea");
//                var overlay = mask ? mask.Find("CooldownOverlay") : null;
//                var img = overlay ? overlay.GetComponent<Image>() : null;
//                if (img)
//                {
//                    var cui = s.GetComponent<SkillCooldownUI>();
//                    if (!cui) cui = s.gameObject.AddComponent<SkillCooldownUI>();
//                    var so = new SerializedObject(cui);
//                    so.FindProperty("cooldownOverlay").objectReferenceValue = img;
//                    so.ApplyModifiedPropertiesWithoutUndo();

//                    s.cooldownUI = cui;

//                    // ★ 쿨오버레이 세팅: Type=Filled, FillMethod=Radial360, FillOrigin=Top, Clockwise 등
//                    img.type = Image.Type.Filled;
//                    img.fillMethod = Image.FillMethod.Radial360;
//                    img.fillOrigin = 2; // Top
//                    img.fillClockwise = false;
//                    img.fillAmount = 0f;
//                }
//            }
//        }
//        Debug.Log($"[SkillQuickBar] AutoWireSlots OK (slots={slots?.Length ?? 0})");
//    }

//    public void Assign(int index, string skillId, Sprite icon)
//    {
//        if (index < 0 || index >= slots.Length) return;
//        slots[index].SetSkill(skillId, icon);
//    }

//    public bool AssignToFirstEmpty(string skillId, Sprite icon)
//    {
//        for (int i = 0; i < slots.Length; i++)
//            if (string.IsNullOrEmpty(slots[i].SkillId)) { Assign(i, skillId, icon); return true; }
//        return false;
//    }

//    public void Swap(int a, int b)
//    {
//        if (a == b) return;
//        if (a < 0 || b < 0 || a >= slots.Length || b >= slots.Length) return;
//        var tmp = slots[a].GetData();
//        slots[a].ApplyData(slots[b].GetData());
//        slots[b].ApplyData(tmp);
//    }

//    public string GetSkillAt(int index)
//    {
//        if (index < 0 || index >= slots.Length) return null;
//        return slots[index].SkillId;
//    }

//    // ★ SkillManager에서 쿨다운UI 접근할 때 사용
//    public SkillSlotUI GetSlot(int index)
//    {
//        if (index < 0 || index >= (slots?.Length ?? 0)) return null;
//        return slots[index];
//    }
//}
// SkillQuickBar.cs
// SkillQuickBar.cs (추가/수정)
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillQuickBar : MonoBehaviour
{
    public SkillSlotUI[] slots;

    public event Action OnChanged; // ← 슬롯 구성 바뀔 때 저장 트리거

    public void AutoWireSlots()
    {
        if (slots == null || slots.Length == 0)
            slots = GetComponentsInChildren<SkillSlotUI>(true);

        for (int i = 0; i < slots.Length; i++)
        {
            var s = slots[i];
            if (!s) continue;

            s.index = i;

            // 아이콘 자동 연결
            if (!s.icon)
            {
                var iconTr = s.transform.Find("A");
                var img = iconTr ? iconTr.GetComponent<Image>() : null;
                if (!img) img = s.GetComponentInChildren<Image>(true);
                if (img) { s.icon = img; s.icon.raycastTarget = false; }
            }

            // 쿨다운 자동 연결
            if (!s.cooldownUI)
            {
                var cui = s.GetComponent<SkillCooldownUI>();
                if (!cui) cui = s.gameObject.AddComponent<SkillCooldownUI>();
                s.cooldownUI = cui;
            }
            var mask = s.transform.Find("MaskArea");
            var overlay = mask ? mask.Find("CooldownOverlay") : null;
            var overlayImg = overlay ? overlay.GetComponent<Image>() : null;
            if (overlayImg) s.cooldownUI.BindOverlay(overlayImg);
        }
    }

    public void Assign(int index, string skillId, Sprite icon)
    {
        if (index < 0 || index >= slots.Length) return;
        slots[index].SetSkill(skillId, icon);
        OnChanged?.Invoke(); // 저장 트리거
    }

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

    public void Swap(int a, int b)
    {
        if (a == b) return;
        if (a < 0 || b < 0 || a >= slots.Length || b >= slots.Length) return;
        var tmp = slots[a].GetData();
        slots[a].ApplyData(slots[b].GetData());
        slots[b].ApplyData(tmp);
        OnChanged?.Invoke(); // 저장 트리거
    }

    public string GetSkillAt(int index)
    {
        if (index < 0 || index >= slots.Length) return null;
        return slots[index].SkillId;
    }

    // ==== JSON으로 내보내기/불러오기 ====
    public QuickBarSave ToSaveData()
    {
        var data = new QuickBarSave();
        for (int i = 0; i < slots.Length; i++)
        {
            var id = slots[i].SkillId;
            if (!string.IsNullOrEmpty(id))
                data.slots.Add(new SlotEntry { index = i, skillId = id });
        }
        return data;
    }

    /// <summary>
    /// 저장 데이터를 적용. iconResolver로 아이콘 주입.
    /// canUse(skillId)로 언락 여부 검증(언락 안 된건 무시).
    /// </summary>
    public void ApplySaveData(
        QuickBarSave save,
        Func<string, Sprite> iconResolver,
        Func<string, bool> canUse)
    {
        if (save == null) return;

        // 전체 초기화(원하면 유지 가능)
        for (int i = 0; i < slots.Length; i++)
            slots[i].SetSkill(null, null);

        foreach (var e in save.slots)
        {
            if (e.index < 0 || e.index >= slots.Length) continue;
            if (string.IsNullOrEmpty(e.skillId)) continue;

            // 언락 검증(치트/구버전 보호)
            if (canUse != null && !canUse(e.skillId)) continue;

            var sp = iconResolver?.Invoke(e.skillId);
            slots[e.index].SetSkill(e.skillId, sp);
        }

        OnChanged?.Invoke(); // 적용 후 저장(선택)
    }

    public SkillSlotUI GetSlot(int index)
    {
        if (index < 0 || index >= (slots?.Length ?? 0)) return null;
        return slots[index];
    }
}
