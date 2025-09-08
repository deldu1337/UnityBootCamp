using UnityEngine;
using UnityEngine.UI;

public class MinimapCamera : MonoBehaviour
{
    private Camera minimapCam; // Minimap ī�޶� ����
    private Image arrowImage;
    private Vector3 distance; // ī�޶�� �÷��̾� �� ��� ��ġ (offset)
    private Vector3 fixedRotation = new Vector3(90f, 45f, 0f); // �̴ϸ� ī�޶� ���� ȸ����

    void Start()
    {
        // Minimap �±׸� ���� ī�޶� ã�Ƽ� ����
        minimapCam = GameObject.FindGameObjectWithTag("Minimap").GetComponent<Camera>();
        if (minimapCam == null)
        {
            Debug.LogError("Minimap �±׸� ���� ī�޶� ã�� �� �����ϴ�!");
            return;
        }
        Transform canvasTransform = transform.Find("Canvas");
        if (canvasTransform != null)
        {
            Transform arrowTransform = canvasTransform.Find("ArrowImage");
            if (arrowTransform != null)
            {
                arrowImage = arrowTransform.GetComponent<Image>();
            }
        }

        // ī�޶� ȸ�� ����
        minimapCam.transform.eulerAngles = fixedRotation;

        // �ʱ� ī�޶� ��ġ�� �������� offset ���
        Vector3 vector3 = new Vector3(transform.position.x, 50f, transform.position.z); // ī�޶� �ʱ� ��ġ
        distance = vector3 - transform.position;       // �÷��̾� ��ġ ���� offset ���
       
        // �ּ� ó����, ������ �ʱ� ���ͷ� offset ����
    }

    void FixedUpdate()
    {
        // ī�޶� ��ġ�� �÷��̾� ��ġ + offset���� ����
        minimapCam.transform.position = distance + transform.position;

        // ���� ī�޶� ��ġ ���� offset ����
        distance = minimapCam.transform.position - transform.position;

        // �÷��̾� y�� ȸ��(����) �������� UI Z�� ȸ��
        float playerYRotation = transform.eulerAngles.y;
        arrowImage.rectTransform.localEulerAngles = new Vector3(45f, 0f, -playerYRotation + 45f);
        // 45�� �ν����Ϳ��� ������ x�� ����, y�� 0���� ����, z�� �÷��̾� ȸ�� �ݿ�
    }
}