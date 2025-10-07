using System;
using System.Text;
using UnityEngine;

public static class ItemStatCompare
{
    // UI ���� (Unity Text ��ġ�ؽ�Ʈ)
    private const string POS = "#35C759"; // �ʷ�
    private const string NEG = "#FF3B30"; // ����
    private const string ZERO = "#A1A1A6"; // ȸ��
    private const string LABEL = "#DADADA"; // �� �ؽ�Ʈ �⺻��
    private const string MAX = "#49DDDF";  // �ؿ� ���̶���Ʈ(���гΰ� ���� ��)

    // inv > eq �̸� �ʷ� +, inv < eq �̸� ���� -, 0�̸� ȸ�� 0
    private static string ColorizeDiff(float diff, bool signed = true)
    {
        string sign = diff > 0.0001f ? "+" : (diff < -0.0001f ? "" : "");
        string color = diff > 0.0001f ? POS : (diff < -0.0001f ? NEG : ZERO);
        string val = diff.ToString(diff == Mathf.RoundToInt(diff) ? "+0;-#;0" : "+0.##;-0.##;0");
        if (!signed) val = diff.ToString(diff == Mathf.RoundToInt(diff) ? "0;-#;0" : "0.##;-0.##;0");
        return $"<color={color}>{(signed ? val : sign + val)}</color>";
    }

    private static float Eff(float baseVal, bool hasRolled, float rolledVal)
        => hasRolled ? rolledVal : baseVal;

    private static void Take(InventoryItem item, out float hp, out float mp, out float atk, out float def, out float dex, out float AS, out float cc, out float cd)
    {
        // rolled�� ������ has* �÷��׸� ���� �켱 ���
        if (item.rolled != null)
        {
            hp = Eff(item.data.hp, item.rolled.hasHp, item.rolled.hp);
            mp = Eff(item.data.mp, item.rolled.hasMp, item.rolled.mp);
            atk = Eff(item.data.atk, item.rolled.hasAtk, item.rolled.atk);
            def = Eff(item.data.def, item.rolled.hasDef, item.rolled.def);
            dex = Eff(item.data.dex, item.rolled.hasDex, item.rolled.dex);
            AS = Eff(item.data.As, item.rolled.hasAs, item.rolled.As);
            cc = Eff(item.data.cc, item.rolled.hasCc, item.rolled.cc);
            cd = Eff(item.data.cd, item.rolled.hasCd, item.rolled.cd);
        }
        else
        {
            hp = item.data.hp; mp = item.data.mp; atk = item.data.atk; def = item.data.def;
            dex = item.data.dex; AS = item.data.As; cc = item.data.cc; cd = item.data.cd;
        }
    }

    public static string BuildSingleTooltip(InventoryItem item, string tier = null)
    {
        Take(item, out var hp, out var mp, out var atk, out var def, out var dex, out var AS, out var cc, out var cd);

        string TierLine = string.IsNullOrEmpty(tier) ? "" : $"\n<color=#FFD60A>[{tier}]</color>";
        return
$@"<b>{item.data.name}</b>{TierLine}
<color={LABEL}>����</color>  {item.data.type}
<color={LABEL}>����</color>  {Mathf.Max(1, item.data.level)}

<color={LABEL}>HP</color>  {hp}
<color={LABEL}>MP</color>  {mp}
<color={LABEL}>ATK</color> {atk}
<color={LABEL}>DEF</color> {def}
<color={LABEL}>DEX</color> {dex}
<color={LABEL}>AS</color>  {AS}
<color={LABEL}>CC</color>  {cc}
<color={LABEL}>CD</color>  {cd}";
    }

    // inv �������� �ش� ���� "�ɼ��� ���� �ִ���" ���� (���� �гΰ� ������ ö��)
    private static bool HasOption(InventoryItem item, string key, float baseVal, out float effective)
    {
        effective = baseVal;
        var r = item.rolled;
        bool hasRolled =
            r != null && r.TryGet(key, out effective); // �Ѹ� �� �켱

        // ��ȿ��: �Ѹ� ���� ������ �� ��, ������ base ���� 0�� �ƴ����� �Ǵ�
        const float EPS = 0.0001f;
        float v = hasRolled ? effective : baseVal;
        return Mathf.Abs(v) > EPS;
    }

    private static string FormatValue(string key, float v)
    {
        if (key == "cc") return $"{v * 100f:0.##}%";
        if (key == "cd") return $"{v:0.##}";
        return $"{v:0.##}";
    }

    /// <summary>
    /// �κ� �������� ���� �ɼǸ� ���.
    /// showEquippedValues=true �� �� �� ���� "����(eq) ��"�� ǥ���ϰ�,
    /// ��ȣ���� �׻� (�κ� - ����) �������� ǥ��.
    /// </summary>
    public static string BuildCompareLines(InventoryItem inv, InventoryItem eq, bool showEquippedValues)
    {
        // ���� ȿ����(rolled �ݿ�) ��������
        Take(inv, out var hp1, out var mp1, out var atk1, out var def1, out var dex1, out var AS1, out var cc1, out var cd1);
        Take(eq, out var hp0, out var mp0, out var atk0, out var def0, out var dex0, out var AS0, out var cc0, out var cd0);

        string Line(string label, string key, float invV, float eqV)
        {
            float diff = invV - eqV;                         // (�κ� - ����)
            float shown = showEquippedValues ? eqV : invV;   // ǥ�ð� ����

            // �ؿ� ������ "ǥ�� ��ü" �����ۿ��� ���� (���гΰ� ������ ö��)
            InventoryItem shownItem = showEquippedValues ? eq : inv;
            bool isMax = false;
            if (shownItem != null && shownItem.data != null && shownItem.data.type != "potion")
            {
                // key�� shown������ �ؿ� ����
                isMax = ItemRoller.IsMaxRoll(shownItem.id, key, shown);
            }

            string valueStr = FormatValue(key, shown);
            if (isMax) valueStr = $"<color={MAX}>{valueStr}</color>";

            return $"<color={LABEL}>{label}</color>  {valueStr}  ({ColorizeDiff(diff)})";
        }

        var sb = new StringBuilder();
        float _;
        if (HasOption(inv, "hp", inv.data.hp, out _)) sb.AppendLine(Line("HP", "hp", hp1, hp0));
        if (HasOption(inv, "mp", inv.data.mp, out _)) sb.AppendLine(Line("MP", "mp", mp1, mp0));
        if (HasOption(inv, "atk", inv.data.atk, out _)) sb.AppendLine(Line("������", "atk", atk1, atk0));
        if (HasOption(inv, "def", inv.data.def, out _)) sb.AppendLine(Line("����", "def", def1, def0));
        if (HasOption(inv, "dex", inv.data.dex, out _)) sb.AppendLine(Line("��ø��", "dex", dex1, dex0));
        if (HasOption(inv, "As", inv.data.As, out _)) sb.AppendLine(Line("���� �ӵ�", "As", AS1, AS0));
        if (HasOption(inv, "cc", inv.data.cc, out _)) sb.AppendLine(Line("ġ��Ÿ Ȯ��", "cc", cc1, cc0));
        if (HasOption(inv, "cd", inv.data.cd, out _)) sb.AppendLine(Line("ġ��Ÿ ������", "cd", cd1, cd0));

        if (sb.Length == 0) sb.Append("ǥ���� �ɼ� ����");
        return sb.ToString();
    }
}
