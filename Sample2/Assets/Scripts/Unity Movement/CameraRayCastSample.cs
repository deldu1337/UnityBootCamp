using UnityEngine;
// 카메라 기준으로 마우스 클릭 위치에 레이캐스트 처리

public class CameraRayCastSample : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // Ray ray = new Ray(위치, 방향);

            // 카메라에서 사용할 레이를 따로 설정
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("넌 이제 노랑색이야!");
                hit.collider.GetComponent<Renderer>().material.color = Color.yellow;

                var hitObject = hit.collider.gameObject;

                int change_layer = LayerMask.NameToLayer("Yellow");

                // 레이어가 유효한 값일 경우
                if (change_layer != -1)
                {
                    hitObject.layer = change_layer;
                }
            }
        }
    }
}
