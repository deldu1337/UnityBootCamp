using System;
using System.Collections.Generic;

[Serializable]
public class PotionQuickBarSave
{
    public List<PotionSlotEntry> slots = new(); // index, uid, id, iconPath, prefabPath, hp, mp
}

[Serializable]
public class PotionSlotEntry
{
    public int index;
    public string uniqueId;     // �κ� UID (�����Կ� �̰��� �Ŀ��� ����)
    public int itemId;          // DataManager�� id (�籸����)
    public string iconPath;     // Resources ��� (��: "Icons/HPPotion")
    public string prefabPath;   // ���� ������ ���(������)
    public float hp;
    public float mp;

    // �� �߰�: ����
    public int qty;
}
