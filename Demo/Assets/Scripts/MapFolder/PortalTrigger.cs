//using UnityEngine;

//[RequireComponent(typeof(Collider))]
//public class PortalTrigger : MonoBehaviour
//{
//    [Header("�÷��̾� ���̾� ����")]
//    [SerializeField] private LayerMask playerLayer;  // �� �ν����Ϳ��� Player ���̾� üũ

//    private TileMapGenerator generator;

//    public void Setup(TileMapGenerator owner)
//    {
//        generator = owner;
//    }

//    private void Reset()
//    {
//        // �����Ϳ��� ���� �� �ڵ����� Ʈ����ȭ
//        var col = GetComponent<Collider>();
//        if (col != null) col.isTrigger = true;

//        // �⺻������ "Player" ���̾ �ִٸ� �ڵ� ���� �õ�
//        int idx = LayerMask.NameToLayer("Player");
//        if (idx >= 0)
//        {
//            playerLayer = 1 << idx;
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        // ���̾� ����ũ�� �浹ü�� �÷��̾����� ����
//        bool isPlayer = (playerLayer.value & (1 << other.gameObject.layer)) != 0;

//        if (isPlayer && generator != null)
//        {
//            generator.ReloadMap();
//        }
//    }
//}

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PortalTrigger : MonoBehaviour
{
    [Header("�÷��̾� ���̾�")]
    [SerializeField] private LayerMask playerLayer;

    private StageManager stageManager;

    public void Setup(TileMapGenerator owner)
    {
        // owner�� �� �ᵵ ������, �ʿ�� ���� ����
        stageManager = FindAnyObjectByType<StageManager>();
    }

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;

        int idx = LayerMask.NameToLayer("Player");
        if (idx >= 0) playerLayer = 1 << idx;
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = (playerLayer.value & (1 << other.gameObject.layer)) != 0;
        if (!isPlayer) return;

        if (stageManager != null) stageManager.NextStage();
    }
}
