//using UnityEngine;
//using UnityEngine.UI;

//public class PlayerInfoPresenter : MonoBehaviour
//{
//    [SerializeField] private GameObject playerInfoUI;
//    [SerializeField] private Button exitButton;
//    private EquipmentPresenter equipmentPresenter;

//    [SerializeField] private GameObject equipmentUI;

//    [SerializeField] private bool forceCloseOnStart = true;

//    private RectTransform playerInfoRect;
//    private RectTransform equipmentRect;

//    private bool isOpen = false;
//    public bool IsOpen => isOpen;

//    // 비활성 포함 탐색 헬퍼
//    private static GameObject FindIncludingInactive(string name)
//    {
//        var all = Resources.FindObjectsOfTypeAll<GameObject>();
//        for (int i = 0; i < all.Length; i++)
//        {
//            var go = all[i];
//            if (go && go.name == name && (go.hideFlags == 0))
//                return go;
//        }
//        return null;
//    }

//    void Start()
//    {
//        if (!playerInfoUI) playerInfoUI = GameObject.Find("PlayerInfoUI") ?? FindIncludingInactive("PlayerInfoUI");
//        if (!equipmentUI) equipmentUI = GameObject.Find("EquipmentUI") ?? FindIncludingInactive("EquipmentUI");

//        UIEscapeStack.GetOrCreate();

//        if (!playerInfoUI)
//        {
//            Debug.LogError("[PlayerInfoPresenter] playerInfoUI를 찾지 못했습니다.");
//            enabled = false;
//            return;
//        }

//        // EquipmentPresenter/EquipmentView 경유로 Rect를 얻는 루트
//        if (!equipmentPresenter)
//            equipmentPresenter = FindAnyObjectByType<EquipmentPresenter>(FindObjectsInactive.Include);

//        playerInfoRect = playerInfoUI.GetComponent<RectTransform>();

//        if (!equipmentUI && equipmentPresenter != null)
//        {
//            // View.RootRect 경유
//            var view = FindAnyObjectByType<EquipmentView>(FindObjectsInactive.Include);
//            if (view && view.RootRect)
//                equipmentUI = view.RootRect.gameObject;
//        }

//        equipmentRect = equipmentUI ? equipmentUI.GetComponent<RectTransform>() : null;

//        if (exitButton) exitButton.onClick.AddListener(Close);

//        if (forceCloseOnStart)
//        {
//            if (playerInfoUI.activeSelf) playerInfoUI.SetActive(false);
//            isOpen = false;
//            UIEscapeStack.Instance.Remove("playerinfo");
//        }
//        else
//        {
//            isOpen = playerInfoUI.activeSelf;
//            if (isOpen) UIEscapeStack.Instance.Push("playerinfo", Close, () => isOpen);
//        }
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.R))
//        {
//            // ★ 전환 직전: 장비창이 켜져 있으면 현재 위치 스냅샷 저장
//            bool equipWasOpen = equipmentPresenter && equipmentPresenter.IsOpen;
//            if (equipWasOpen && equipmentRect)
//                UIPanelSwitcher.SaveSnapshot(equipmentRect);

//            // 기존 로직 유지
//            if (equipWasOpen && equipmentRect && playerInfoRect)
//                UIPanelSwitcher.CopyLayoutRT(equipmentRect, playerInfoRect);

//            Toggle();

//            if (equipWasOpen && equipmentPresenter)
//            {
//                UIPanelSwitcher.CopyLayoutRT(playerInfoRect, equipmentRect);
//                equipmentPresenter.CloseEquipmentPublic();
//            }
//        }
//    }

//    public void Open()
//    {
//        if (isOpen || !playerInfoUI) return;

//        // ★ 열리기 전에 스냅샷 있으면 복원
//        if (playerInfoRect && UIPanelSwitcher.HasSnapshot)
//            UIPanelSwitcher.LoadSnapshot(playerInfoRect);

//        if (equipmentUI && equipmentUI.activeSelf) equipmentUI.SetActive(false);

//        playerInfoUI.SetActive(true);
//        isOpen = true;
//        UIEscapeStack.Instance.Push("playerinfo", Close, () => isOpen);
//    }

//    public void Close()
//    {
//        if (!isOpen || !playerInfoUI) return;

//        // ★ 닫히기 직전 현재 위치 스냅샷 저장
//        if (playerInfoRect)
//            UIPanelSwitcher.SaveSnapshot(playerInfoRect);

//        playerInfoUI.SetActive(false);
//        isOpen = false;
//        UIEscapeStack.Instance.Remove("playerinfo");
//    }


//    public void Toggle() { if (isOpen) Close(); else Open(); }
//}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInfoPresenter : MonoBehaviour
{
    [SerializeField] private GameObject playerInfoUI;
    [SerializeField] private Button exitButton;

    [SerializeField] private GameObject equipmentUI;
    [SerializeField] private bool forceCloseOnStart = true;

    private Button InfoButton;
    private Image image;
    private Sprite[] sprites;
    private RectTransform playerInfoRect;   // ★ 실제로 움직이는 RT
    private RectTransform equipmentRect;    // ★ 실제로 움직이는 RT
    private bool isOpen = false;
    public bool IsOpen => isOpen;

    // 비활성 포함 탐색 (이름으로)
    private static GameObject FindIncludingInactive(string name)
    {
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < all.Length; i++)
        {
            var go = all[i];
            if (go && go.name == name && (go.hideFlags == 0))
                return go;
        }
        return null;
    }

    // 루트에서 실제 드래그로 이동하는 "창 패널" RT 찾기
    // 규칙: 루트 하위에서 UIDragHandler를 찾고, 그 핸들러의 parent RT를 반환. 없으면 루트의 RT.
    // ★ 공통: 실제로 움직일 "창 루트" RT를 얻는다.
    // 규칙:
    //  1) root 하위에서 이름이 "HeadPanel"인 트랜스폼을 찾고 -> 그 parent RT를 창 루트로 사용
    //  2) 없으면 root의 RectTransform 사용
    //  3) 마지막으로 Canvas의 '직계 자식' 레벨까지 타고 올라가 통일
    private static RectTransform GetMovableWindowRT(GameObject root)
    {
        if (!root) return null;

        RectTransform cand = null;

        // 1) HeadPanel 우선
        var head = root.transform.Find("HeadPanel");
        if (head == null)
        {
            // 혹시 더 깊이 있을 수 있으니 전체 탐색
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == "HeadPanel") { head = t; break; }
            }
        }

        if (head && head.parent is RectTransform headParentRT)
        {
            cand = headParentRT; // HeadPanel의 부모가 '창 루트'
        }
        else
        {
            // 2) fallback: root의 RT
            cand = root.GetComponent<RectTransform>();
        }

        if (!cand) return null;

        // 3) Canvas 직계 자식 레벨까지 끌어올리기(양쪽 패널을 동일 레벨로 통일)
        RectTransform cur = cand;
        while (cur && cur.parent is RectTransform prt)
        {
            if (prt.GetComponent<Canvas>() != null) break; // prt가 Canvas → cur는 Canvas 직계
            cur = prt;
        }
        return cur;
    }

    void Start()
    {
        if (!playerInfoUI) playerInfoUI = GameObject.Find("PlayerInfoUI") ?? FindIncludingInactive("PlayerInfoUI");
        if (!equipmentUI) equipmentUI = GameObject.Find("EquipmentUI") ?? FindIncludingInactive("EquipmentUI");
        exitButton = playerInfoUI.transform.GetChild(4).GetComponent<Button>();
        sprites = new Sprite[8];
        sprites = Resources.LoadAll<Sprite>("CharacterIcons");
        

        var ps = PlayerStatsManager.Instance;
        string race = (ps != null && ps.Data != null && !string.IsNullOrEmpty(ps.Data.Race))
                        ? ps.Data.Race
                        : "humanmale";

        var quickUI = GameObject.Find("QuickUI");
        if (quickUI != null && quickUI.transform.childCount > 1)
        {
            InfoButton = quickUI.transform.GetChild(2).GetComponent<Button>();
            if (InfoButton) InfoButton.onClick.AddListener(Toggle);
        }

        image = InfoButton.GetComponent<Image>();

        for (int i = 0; i < sprites.Length; i++)
        {
            Debug.Log(sprites[i].name);
            if(sprites[i].name == race)
                image.sprite = sprites[i];
        }

        UIEscapeStack.GetOrCreate();

        if (!playerInfoUI)
        {
            Debug.LogError("[PlayerInfoPresenter] playerInfoUI를 찾지 못했습니다.");
            enabled = false;
            return;
        }

        // 실제 움직이는 RT로 세팅
        playerInfoRect = GetMovableWindowRT(playerInfoUI);
        equipmentRect = GetMovableWindowRT(equipmentUI);

        if (exitButton) exitButton.onClick.AddListener(Close);

        if (forceCloseOnStart)
        {
            if (playerInfoUI.activeSelf) playerInfoUI.SetActive(false);
            isOpen = false;
            UIEscapeStack.Instance.Remove("playerinfo");
        }
        else
        {
            isOpen = playerInfoUI.activeSelf;
            if (isOpen) UIEscapeStack.Instance.Push("playerinfo", Close, () => isOpen);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 전환 직전: 장비창이 켜져 있으면, 장비창의 "움직이는 RT" 기준으로 스냅샷 저장 + 레이아웃 복사
            var eqPresenter = FindAnyObjectByType<EquipmentPresenter>();
            bool equipWasOpen = eqPresenter && eqPresenter.IsOpen;

            if (equipWasOpen && equipmentRect)
            {
                Debug.Log($"[SNAP] Save from: {PathOf(playerInfoRect)} localPos={playerInfoRect.localPosition}");
                UIPanelSwitcher.SaveSnapshot(equipmentRect);
            }

            if (equipWasOpen && equipmentRect && playerInfoRect)
                UIPanelSwitcher.CopyLayoutRT(equipmentRect, playerInfoRect);

            Toggle();

            if (equipWasOpen && eqPresenter)
            {
                eqPresenter.CloseEquipmentPublic();

                // (선택) 현재 PI 위치를 장비쪽에도 반영해두고 닫기
                if (playerInfoRect && equipmentRect)
                    UIPanelSwitcher.CopyLayoutRT(playerInfoRect, equipmentRect);
            }
        }
    }

    private static string PathOf(Transform t)
    {
        if (!t) return "<null>";
        System.Text.StringBuilder sb = new System.Text.StringBuilder(t.name);
        while (t.parent)
        {
            t = t.parent;
            sb.Insert(0, t.name + "/");
        }
        return sb.ToString();
    }

    public void Toggle() { if (isOpen) Close(); else Open(); }

    // PlayerInfoPresenter
    public void Open()
    {
        if (isOpen || !playerInfoUI) return;

        // 1차 적용(먹을 때도 있음)
        if (playerInfoRect && UIPanelSwitcher.HasSnapshot)
        {
            Debug.Log($"[SNAP] Load  to: {PathOf(playerInfoRect)}");
            UIPanelSwitcher.LoadSnapshot(playerInfoRect);
        }

        playerInfoUI.SetActive(true);
        isOpen = true;
        UIEscapeStack.Instance.Push("playerinfo", Close, () => isOpen);

        // ★ 핵심: 활성화로 인한 레이아웃 리빌드가 끝난 "다음 프레임"에 다시 복원
        if (playerInfoRect && UIPanelSwitcher.HasSnapshot)
            StartCoroutine(ReapplySnapshotNextFrame(playerInfoRect));
    }

    private System.Collections.IEnumerator ReapplySnapshotNextFrame(RectTransform rt)
    {
        yield return null; // 한 프레임 대기 (SetActive → 부모 레이아웃 리빌드 끝난 뒤)
        Debug.Log($"[SNAP] Load  to: {PathOf(playerInfoRect)}");
        UIPanelSwitcher.LoadSnapshot(rt);           // 스냅샷 재적용
        Canvas.ForceUpdateCanvases();
        var prt = rt.parent as RectTransform;
        if (prt) UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(prt);
    }


    public void Close()
    {
        if (!isOpen || !playerInfoUI) return;

        // 닫히기 직전 현재 위치 스냅샷 저장 (움직이는 RT)
        if (playerInfoRect)
            UIPanelSwitcher.SaveSnapshot(playerInfoRect);

        playerInfoUI.SetActive(false);
        isOpen = false;
        UIEscapeStack.Instance.Remove("playerinfo");
    }
}
