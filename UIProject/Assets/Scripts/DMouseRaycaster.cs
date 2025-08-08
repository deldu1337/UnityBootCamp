using UnityEngine;

public class DMouseRaycaster : MonoBehaviour
{
    private Camera cam;
    public float distance = 10.0f;
    public LayerMask layerMask;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay( Input.mousePosition );

            if (Physics.Raycast(ray, out RaycastHit hit, distance, layerMask))
            {
                // 트리거 체크
                var trigger = hit.collider.GetComponent<DTrigger>();

                if (trigger != null)
                {
                    trigger.OnTriggerEnter();
                    // 트리거를 통한 다이얼로그 접근 코드 작성
                }
            }
        }
    }
}
