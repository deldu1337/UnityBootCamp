using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using static GLTFast.Schema.AnimationChannelBase;
using Path = System.IO.Path;

public class ItemPickup : MonoBehaviour
{
    [TextArea] public string itemInfo; // ������ ����
    [TextArea] public string id; // ������ id

    [Header("���� ����")]
    public float showDistance = 7f; // �÷��̾�� �� �Ÿ� �ȿ����� ���� ǥ��

    private bool isMouseOver = false;
    private Transform player;
    private DataManager dataManager;
    private string inventoryFilePath;

    private void Start()
    {
        // ������ name �ҷ�����
        dataManager = DataManager.GetInstance();
        dataManager.LoadDatas();

        // C:\Users\user\AppData\LocalLow\DefaultCompany\Demo
        inventoryFilePath = Path.Combine(Application.persistentDataPath, "playerInventory.json");

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

                // ��Ŭ�� �� ������ ����
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
