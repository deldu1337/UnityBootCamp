using UnityEngine;
using UnityEngine.UI;

public class InterPlayerSample : MonoBehaviour
{

    //�ν����� ������ ���� ����(���� ������ ���� ����)
    //�ܺο��� ���� �Ұ�(�Ժη� �� ���� ����� �뵵)
    [SerializeField] private ScriptableObject attackObject;

    public Button action01;
    public Button action02;
    public Button action03;

    private IAttackStrategySample strategy;
    private void Awake()
    {
        strategy = attackObject as IAttackStrategySample;

        if (strategy == null)
        {
            Debug.LogError("���� ����� �������� �ʾѽ��ϴ�!");
        }

        if (attackObject is MeleeAttackSample)
        {
            action01.interactable = true;
            Debug.Log("�� ĳ���ʹ� ���� ĳ�����Դϴ�.");
        }
        if (attackObject is RangedAttackSample)
        {
            action02.interactable = true;
            Debug.Log("�� ĳ���ʹ� ���Ÿ� ĳ�����Դϴ�.");
        }
        if (attackObject is CastedAttackSample)
        {
            action03.interactable = true;
            Debug.Log("�� ĳ���ʹ� ĳ�����Դϴ�.");
        }


    }
    public void ActionPerformed(GameObject target)
    {
        strategy?.Attack(gameObject, target);
        //Nullable<T> or T? �� Value�� ���� null ����� ���� ����
    }
}