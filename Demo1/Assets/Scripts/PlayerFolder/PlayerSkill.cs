using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private Animation animationComponent; // Animator ���

    void Awake()
    {
        animationComponent = GetComponent<Animation>();

        if (animationComponent == null)
            Debug.LogError("Animation ������Ʈ�� Player ������ �Ǵ� �ڽĿ� �����ϴ�!");
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
