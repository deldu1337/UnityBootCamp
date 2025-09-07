using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private Animation animationComponent; // Animator 대신

    void Awake()
    {
        animationComponent = GetComponent<Animation>();

        if (animationComponent == null)
            Debug.LogError("Animation 컴포넌트가 Player 프리팹 또는 자식에 없습니다!");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            if (animationComponent != null)
                animationComponent.CrossFade("Whirlwind (ID 126 variation 0)", 0.1f);
        }
    }
}
