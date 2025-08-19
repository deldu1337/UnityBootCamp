using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [Header("Fire Setting")]
    [Tooltip("총알 생산 공장")] public GameObject bulletFactory;
    [Tooltip("총구")] public GameObject firePosition;

    private void Update()
    {
        // GetKeyXXX : KeyCode에 등록되어있는 키 입력
        // GetButtonXXX : Axes 키에 대한 입력
        // GetMouseButtonXXX : 마우스 클릭에 대한 설정 0,1,2



        // Input Manager의 FIre1 키에 대한 입력이 진행되었을 경우 발사를 진행한다.
        if(Input.GetButtonDown("Fire1"))
        {
            // 총알은 총알 생산 공장에서 등록한 총알을 생성한다.
            // 총알의 위치는 총구 지점으로 설정한다.
            // 별도의 회전은 넣지 않는다.
            var bullet = Instantiate(bulletFactory, firePosition.transform.position, Quaternion.identity);
        }
    }
}
