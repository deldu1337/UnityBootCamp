using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using static GLTFast.Schema.AnimationChannelBase;
using Path = System.IO.Path;

public class ItemPickup : MonoBehaviour
{
    [TextArea] public string itemInfo; // 아이템 설명
    [TextArea] public string id; // 아이템 id

    [Header("툴팁 설정")]
    public float showDistance = 7f; // 플레이어와 이 거리 안에서만 툴팁 표시

    private bool isMouseOver = false;
    private Transform player;
    private DataManager dataManager;
    private string inventoryFilePath;

    private void Start()
    {
        // 아이템 name 불러오기
        dataManager = DataManager.GetInstance();
        dataManager.LoadDatas();

        // C:\Users\user\AppData\LocalLow\DefaultCompany\Demo
        inventoryFilePath = Path.Combine(Application.persistentDataPath, "playerInventory.json");

        // 플레이어 레이어 기반으로 씬에서 찾기
        int playerLayer = LayerMask.NameToLayer("Player");
        GameObject[] players = FindObjectsOfType<GameObject>();
        foreach (var obj in players)
        {
            if (obj.layer == playerLayer)
            {
                player = obj.transform;
                break;
            }
        }
    }

    private void Update()
    {
        if (isMouseOver && player != null)
        {
            // 플레이어와 아이템 사이 거리 계산
            float distance = Vector3.Distance(player.position, transform.position);

            if (distance <= showDistance)
            {
                ItemTooltip.Instance.Show(dataManager.dicItemDatas[int.Parse(id)].name);

                // 우클릭 시 아이템 제거
                if (Input.GetMouseButtonDown(0))
                {
                    Destroy(gameObject);
                    ItemTooltip.Instance.Hide();
                }
            }
            else
                ItemTooltip.Instance.Hide();
        }
    }

    void OnMouseEnter()
    {
        isMouseOver = true;
        ItemTooltip.Instance.Show(dataManager.dicItemDatas[int.Parse(id)].name);
    }

    void OnMouseExit()
    {
        isMouseOver = false;
        ItemTooltip.Instance.Hide();
    }
}
