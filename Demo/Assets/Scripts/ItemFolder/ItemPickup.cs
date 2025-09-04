using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [TextArea] public string itemInfo; // ������ ����
    [TextArea] public string id; // ������ id
    public Sprite icon; // �κ��丮�� ����� ������

    [Header("���� ����")]
    public float showDistance = 7f; // �÷��̾�� �� �Ÿ� �ȿ����� ���� ǥ��

    private bool isMouseOver = false;
    private Transform player;
    private DataManager dataManager;
    private PlayerInventory playerInventory;

    private void Start()
    {
        dataManager = DataManager.GetInstance();
        dataManager.LoadDatas();

        // �÷��̾� ���̾� ������� ������ ã��
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

        // PlayerInventory �ڵ� ����
        playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory == null)
            Debug.LogError("PlayerInventory�� ã�� �� �����ϴ�! Player ������Ʈ�� PlayerInventory ��ũ��Ʈ�� �ٿ��ּ���.");
    }

    private void Update()
    {
        if (isMouseOver && player != null)
        {
            // �÷��̾�� ������ ���� �Ÿ� ���
            float distance = Vector3.Distance(player.position, transform.position);

            if (distance <= showDistance)
            {
                ItemTooltip.Instance.Show(dataManager.dicItemDatas[int.Parse(id)].name);

                // ��Ŭ�� �� ������ �߰� �� ����
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
