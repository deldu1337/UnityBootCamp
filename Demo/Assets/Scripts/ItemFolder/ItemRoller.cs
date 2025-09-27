using UnityEngine;

public static class ItemRoller
{
    private static readonly string[] Stats = { "hp", "mp", "atk", "def", "dex", "As", "cc", "cd" };

    public static RolledItemStats CreateRolledStats(int itemId)
    {
        var dm = DataManager.Instance;
        if (dm == null) return null;

        var rolled = new RolledItemStats();
        bool any = false;

        foreach (var stat in Stats)
        {
            if (dm.TryGetRange(itemId, stat, out var r) && r != null)
            {
                float v = RollByRule(stat, r.min, r.max);
                rolled.Set(stat, v);
                any = true;
            }
        }

        return any ? rolled : null;
    }

    /// <summary>
    /// hp, mp �� ���� ����
    /// �� ��(atk, def, dex, As, cc, cd) �� 0.1 ����(�Ҽ� ù° �ڸ�)
    /// </summary>
    private static float RollByRule(string stat, float min, float max)
    {
        if (stat == "hp" || stat == "mp")
        {
            int imin = Mathf.CeilToInt(min);
            int imax = Mathf.FloorToInt(max);
            if (imax < imin) imax = imin;
            return Random.Range(imin, imax + 1);
        }
        else
        {
            // 0.1 ����
            // ��: min=1, max=2 �� stepCount = 10 �� {0..10} �� �ϳ� �� 1.0~2.0
            int start = Mathf.RoundToInt(min * 10f);
            int end = Mathf.RoundToInt(max * 10f);
            if (end < start) end = start;
            int k = Random.Range(start, end + 1);
            return Mathf.Round(k / 10f * 10f) / 10f; // �Ҽ�1�ڸ� ���� ����ȭ
        }
    }

    /// <summary>
    /// ���� ���̶���Ʈ��: value�� ���� stat�� �ִ�ġ�ΰ�?
    /// (0.1 ���� ������ ������ 1�ڸ� �ݿø� ��)
    /// </summary>
    public static bool IsMaxRoll(int itemId, string stat, float value)
    {
        var dm = DataManager.Instance;
        if (dm != null && dm.TryGetRange(itemId, stat, out var r) && r != null)
        {
            if (stat == "hp" || stat == "mp")
            {
                return Mathf.RoundToInt(value) == Mathf.RoundToInt(r.max);
            }
            else
            {
                int v10 = Mathf.RoundToInt(value * 10f);
                int max10 = Mathf.RoundToInt(r.max * 10f);
                return v10 == max10;
            }
        }
        return false;
    }
}
