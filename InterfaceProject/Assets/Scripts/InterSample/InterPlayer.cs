using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class InterPlayer : MonoBehaviour
{
    // �ν����� ������ ���� ����(���� ������ ���� ����) [SerializeField]
    // �ܺο��� ���� �Ұ�(�Ժη� �� ���� ����� �뵵) private
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
            Debug.LogError("���� ����� �������� �ʾҽ��ϴ�!");
        }
        if (Gunner == null)
        {
            Debug.LogError("���� ����� �������� �ʾҽ��ϴ�!");
        }
        if (Mage == null)
        {
            Debug.LogError("���� ����� �������� �ʾҽ��ϴ�!");
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
        // null �� üũ
        if (CheckAttack)
        {
            Warrior?.Attack(target);
            Warrior?.CheckHit(target);
            // Nullable<T> or T? �� Value�� ���� null ����� ���� ����
        }
    }

    public void RangedActionPerformed(GameObject target)
    {
        // null �� üũ
        Gunner?.Attack(target);
        Gunner?.CheckHit(target);
        // Nullable<T> or T? �� Value�� ���� null ����� ���� ����
    }

    public void CastedActionPerformed(GameObject target)
    {
        // null �� üũ
        Mage?.Attack(target);
        Mage?.CheckHit(target);
        // Nullable<T> or T? �� Value�� ���� null ����� ���� ����
    }
}
