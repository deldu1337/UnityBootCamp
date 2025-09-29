using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))] // 캐릭터 - 카메라, 리지드바디, 콜라이더
public class CameraStackHelper : MonoBehaviour
{
   
    void Start()
    {
        Camera mainCamera = Camera.main;
        var mainCameraData = mainCamera.GetUniversalAdditionalCameraData();
        mainCameraData.renderType = CameraRenderType.Base; // 최하위 = 가장 밑 바탕

        Camera uICamera = GetComponent<Camera>(); // UICamera 가져오기
        var uICameraData = uICamera.GetUniversalAdditionalCameraData();
        uICameraData.renderType = CameraRenderType.Overlay;
        mainCameraData.cameraStack.Add(uICamera);

        var uiManagerCamera = UIManager.Instance.UICamera;
        var uiManagerCameraData = uiManagerCamera.GetUniversalAdditionalCameraData();
        uiManagerCameraData.renderType = CameraRenderType.Overlay;
        mainCameraData.cameraStack.Add(uiManagerCamera);
    }

}
