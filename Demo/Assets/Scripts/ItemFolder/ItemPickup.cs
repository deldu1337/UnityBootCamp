using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [TextArea] public string itemInfo; // 아이템 설명
    [TextArea] public string id; // 아이템 id
    public Sprite icon; // 인벤토리에 사용할 아이콘

    [Header("툴팁 설정")]
    public float showDistance = 7f; // 플레이어와 이 거리 안에서만 툴팁 표시

    private bool isMouseOver = false;
    private Transform player;
    private DataManager dataManager;
    private PlayerInventory playerInventory;

    private void Start()
    {
        dataManager = DataManager.GetInstance();
        dataManager.LoadDatas();

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

        // PlayerInventory 자동 참조
        playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory == null)
            Debug.LogError("PlayerInventory를 찾을 수 없습니다! Player 오브젝트에 PlayerInventory 스크립트를 붙여주세요.");
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

                // 우클릭 시 아이템 추가 후 제거
                if (Input.GetMouseButtonDown(0))
                {
                    playerInventory.AddItemToInventory(int.Parse(id), icon);
                    //Debug.Log($"{playerInventory.inv}")
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
