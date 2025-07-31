using UnityEngine;

public class UnityRandom : MonoBehaviour
{
    [ContextMenuItem("랜덤 값 호출", "MenuAttributesMethod")]
    public int rand;

    public void MenuAttributesMethod()
    {
        // 유니티의 랜덤 Random.Range(최소, 최대)
        // 최소 값 범위 포함
        // 최대 값 포함 x (정수)

        // 최소 값 범위 포함
        // 최대 값 포함 o (실수)
        rand = Random.Range(1, 101); // 1 ~ 100
        // 1 ~ 100 중에서 90이하 값이 뽑히는 경우 = 90%
    }
}
