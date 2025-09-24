using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipManager : MonoBehaviour
{
    public static ItemTooltipManager Instance;

    [Header("Prefab & Layout")]
    [SerializeField] private GameObject tooltipPrefab;   // Text + Button ���Ե� UI Prefab
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.1f, 0f);

    // target -> instance
    private readonly Dictionary<Transform, TooltipInstance> actives = new();

    private void Awake()
    {
        Instance = this;
        if (tooltipPrefab == null)
            Debug.LogError("[ItemTooltipManager] tooltipPrefab�� �ʿ��մϴ�.");
    }

    private void LateUpdate()
    {
        // �� ���� ��ġ ���� + �ı��� Ÿ�� ����
        var toRemove = new List<Transform>();
        foreach (var kv in actives)
        {
            var target = kv.Key;
            var inst = kv.Value;

            if (!target || !inst.Panel) { toRemove.Add(target); continue; }

            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + inst.Offset);
            inst.Panel.position = screenPos;
        }

        foreach (var dead in toRemove)
            HideFor(dead);
    }

    public void ShowFor(Transform target, string text, string tier = null, Action onClick = null, Vector3? offset = null)
    {
        if (!target || tooltipPrefab == null) return;
        if (target.GetComponentInParent<EquippedMarker>() != null) return;

        if (!actives.TryGetValue(target, out var inst) || !inst.Panel)
        {
            var go = Instantiate(tooltipPrefab, transform);
            var rect = go.GetComponent<RectTransform>();
            var txt = go.GetComponentInChildren<Text>(true);
            var btn = go.GetComponent<Button>() ?? go.AddComponent<Button>();

            inst = new TooltipInstance
            {
                RootGO = go,
                Panel = rect,
                Text = txt,
                Button = btn
            };
            actives[target] = inst;
        }

        inst.Text.text = text;
        inst.Text.color = GetTierColor(tier);   // �� tier ���� �ݿ�
        inst.Button.onClick.RemoveAllListeners();
        if (onClick != null) inst.Button.onClick.AddListener(() => onClick());

        inst.Offset = offset ?? worldOffset;
        if (!inst.RootGO.activeSelf) inst.RootGO.SetActive(true);
    }


    public void HideFor(Transform target)
    {
        if (!actives.TryGetValue(target, out var inst)) return;

        if (inst.RootGO) Destroy(inst.RootGO);
        actives.Remove(target);
    }

    public void HideAll()
    {
        foreach (var kv in actives)
        {
            if (kv.Value.RootGO) Destroy(kv.Value.RootGO);
        }
        actives.Clear();
    }

    private static Color GetTierColor(string tier)
    {
        if (string.IsNullOrEmpty(tier)) return Color.white;
        switch (tier.ToLower())
        {
            case "common": return Color.white;
            case "uncommon": return new Color32(50, 205, 50, 255);   // ����
            case "rare": return new Color32(0, 128, 255, 255);   // �Ķ�
            case "unique": return new Color32(170, 0, 255, 255);   // ����
            case "legendary": return new Color32(255, 215, 0, 255);   // �ݻ�
            default: return Color.white;
        }
    }

    private class TooltipInstance
    {
        public GameObject RootGO;
        public RectTransform Panel;
        public Text Text;
        public Button Button;
        public Vector3 Offset;
    }
}
