//using UnityEngine;

//[RequireComponent(typeof(Collider))]
//public class PortalTrigger : MonoBehaviour
//{
//    [Header("플레이어 레이어 설정")]
//    [SerializeField] private LayerMask playerLayer;  // ← 인스펙터에서 Player 레이어 체크

//    private TileMapGenerator generator;

//    public void Setup(TileMapGenerator owner)
//    {
//        generator = owner;
//    }

//    private void Reset()
//    {
//        // 에디터에서 붙일 때 자동으로 트리거화
//        var col = GetComponent<Collider>();
//        if (col != null) col.isTrigger = true;

//        // 기본값으로 "Player" 레이어가 있다면 자동 세팅 시도
//        int idx = LayerMask.NameToLayer("Player");
//        if (idx >= 0)
//        {
//            playerLayer = 1 << idx;
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        // 레이어 마스크로 충돌체가 플레이어인지 판정
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
    [Header("플레이어 레이어")]
    [SerializeField] private LayerMask playerLayer;

    private StageManager stageManager;

    public void Setup(TileMapGenerator owner)
    {
        // owner는 안 써도 되지만, 필요시 참조 가능
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
