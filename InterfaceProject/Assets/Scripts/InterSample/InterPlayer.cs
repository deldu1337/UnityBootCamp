using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class InterPlayer : MonoBehaviour
{
    // 인스펙터 내에서 접근 가능(내부 데이터 연결 목적) [SerializeField]
    // 외부에서 접근 불가(함부로 값 쓰지 말라는 용도) private
    [SerializeField] private ScriptableObject MeleeattackObject;
    [SerializeField] private ScriptableObject RangedattackObject;
    [SerializeField] private ScriptableObject CastedattackObject;

    private IAttackStrategy Warrior;
    private IAttackStrategy Gunner;
    private IAttackStrategy Mage;
    private Rigidbody2D Rigidbody;
    private bool CheckAttack = false;

    private void Awake()
    {
        Warrior = MeleeattackObject as IAttackStrategy;
        Gunner = RangedattackObject as IAttackStrategy;
        Mage = CastedattackObject as IAttackStrategy;

        if (Warrior == null)
        {
            Debug.LogError("공격 기능이 구현되지 않았습니다!");
        }
        if (Gunner == null)
        {
            Debug.LogError("공격 기능이 구현되지 않았습니다!");
        }
        if (Mage == null)
        {
            Debug.LogError("공격 기능이 구현되지 않았습니다!");
        }
    }

    private void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        Rigidbody.linearVelocity = new Vector2 (x, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            CheckAttack = true;
        }
    }

    public void MeleeActionPerformed(GameObject target)
    {
        // null 값 체크
        if (CheckAttack)
        {
            Warrior?.Attack(target);
            Warrior?.CheckHit(target);
            // Nullable<T> or T? 는 Value에 대한 null 허용을 위한 도구
        }
    }

    public void RangedActionPerformed(GameObject target)
    {
        // null 값 체크
        Gunner?.Attack(target);
        Gunner?.CheckHit(target);
        // Nullable<T> or T? 는 Value에 대한 null 허용을 위한 도구
    }

    public void CastedActionPerformed(GameObject target)
    {
        // null 값 체크
        Mage?.Attack(target);
        Mage?.CheckHit(target);
        // Nullable<T> or T? 는 Value에 대한 null 허용을 위한 도구
    }
}
