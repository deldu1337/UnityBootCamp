using System.Linq;
using UnityEngine;

/// <summary>
/// 인벤토리와 장비창을 연결하고, 아이템 이동/장착/해제 등의 UI와 모델 업데이트를 관리하는 컨트롤러
/// </summary>
public class InventoryController : MonoBehaviour
{
    // 모델
    public InventoryModel inventory;    // 플레이어 인벤토리 데이터
    public EquipmentModel equipment;    // 장비 슬롯 데이터

    // 뷰
    public InventoryView inventoryView; // 인벤토리 UI
    public EquipmentView equipmentView; // 장비창 UI

    /// <summary>
    /// 시작 시 UI 초기화
    /// </summary>
    private void Start()
    {
        RefreshUI();
    }

    /// <summary>
    /// 인벤토리/장비창 UI 갱신
    /// </summary>
    private void RefreshUI()
    {
        // 인벤토리 UI 갱신
        inventoryView.UpdateInventoryUI(
            inventory.Items,           // 아이템 리스트
            OnItemDropped,             // 드래그앤드롭 이벤트
            OnItemRemoved,             // 인벤토리 외부 삭제 이벤트
            (idx) => OnEquipRequest(idx, ItemOrigin.Inventory) // 장착 요청 이벤트
        );

        // 장비창 UI 갱신
        equipmentView.UpdateEquipmentUI(
            equipment.Slots,                   // 장비 슬롯 리스트
            slotType => OnUnequipRequestBySlot(slotType) // 슬롯 타입 기반 장비 해제 이벤트
        );
    }

    /// <summary>
    /// 인벤토리 아이템 위치 변경 처리 (드래그앤드롭)
    /// </summary>
    /// <param name="from">이동 전 인덱스</param>
    /// <param name="to">이동 후 인덱스</param>
    private void OnItemDropped(int from, int to)
    {
        inventory.SwapItem(from, to); // 모델에서 아이템 위치 교체
        RefreshUI();                  // UI 갱신
    }

    /// <summary>
    /// 인벤토리 아이템 삭제 처리 (인벤토리 외부로 드래그)
    /// </summary>
    /// <param name="index">삭제할 아이템 인덱스</param>
    private void OnItemRemoved(int index)
    {
        inventory.RemoveAt(index); // 모델에서 아이템 제거
        RefreshUI();               // UI 갱신
    }

    /// <summary>
    /// 장비창에서 슬롯 타입 기반 장비 해제 처리
    /// </summary>
    /// <param name="slotType">해제할 장비 슬롯 타입</param>
    private void OnUnequipRequestBySlot(string slotType)
    {
        // 슬롯 타입에 해당하는 인덱스 찾기
        int index = equipment.Slots.ToList().FindIndex(s => s.slotType == slotType);
        if (index < 0) return;

        var item = equipment.Slots[index].equipped;
        if (item == null) return;

        // 인벤토리에 공간이 있으면 장비 해제 후 인벤토리에 추가
        if (inventory.Add(item))
        {
            equipment.Unequip(index); // 모델에서 장비 해제
            Debug.Log($"해제: {item.data.name}");
        }
        RefreshUI(); // UI 갱신
    }

    /// <summary>
    /// 인벤토리에서 장비창으로 장착 요청 처리
    /// </summary>
    /// <param name="index">인벤토리 아이템 인덱스</param>
    /// <param name="origin">아이템 출처 (Inventory/Equipment)</param>
    private void OnEquipRequest(int index, ItemOrigin origin)
    {
        if (origin != ItemOrigin.Inventory) return;

        var item = inventory.Items[index];
        if (item == null) return;

        var slotType = item.data.type;

        // 장비 모델에 장착하고 인벤토리에서 제거
        equipment.EquipItem(slotType, item);
        inventory.RemoveAt(index);
        Debug.Log($"장착: {item.data.name}");

        RefreshUI(); // UI 갱신
    }

    /// <summary>
    /// 장비창에서 인벤토리로 장비 해제 요청 처리
    /// </summary>
    /// <param name="index">장비 슬롯 인덱스</param>
    /// <param name="origin">아이템 출처 (Inventory/Equipment)</param>
    private void OnUnequipRequest(int index, ItemOrigin origin)
    {
        if (origin != ItemOrigin.Equipment) return;

        var item = equipment.Slots[index].equipped;
        if (item == null) return;

        // 인벤토리에 공간이 있으면 장비 해제 후 인벤토리에 추가
        if (inventory.Add(item))
        {
            equipment.Unequip(index); // 모델에서 장비 해제
            Debug.Log($"해제: {item.data.name}");
        }
        RefreshUI(); // UI 갱신
    }
}
