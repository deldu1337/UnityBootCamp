using UnityEngine;
using UnityEngine.UI;

public class InterPlayerSample : MonoBehaviour
{

    //인스펙터 내에서 접근 가능(내부 데이터 연결 목적)
    //외부에서 접근 불가(함부로 값 쓰지 말라는 용도)
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
            Debug.LogError("공격 기능이 구현되지 않앗습니다!");
        }

        if (attackObject is MeleeAttackSample)
        {
            action01.interactable = true;
            Debug.Log("이 캐릭터는 물리 캐릭터입니다.");
        }
        if (attackObject is RangedAttackSample)
        {
            action02.interactable = true;
            Debug.Log("이 캐릭터는 원거리 캐릭터입니다.");
        }
        if (attackObject is CastedAttackSample)
        {
            action03.interactable = true;
            Debug.Log("이 캐릭터는 캐스터입니다.");
        }


    }
    public void ActionPerformed(GameObject target)
    {
        strategy?.Attack(gameObject, target);
        //Nullable<T> or T? 는 Value에 대한 null 허용을 위한 도구
    }
}