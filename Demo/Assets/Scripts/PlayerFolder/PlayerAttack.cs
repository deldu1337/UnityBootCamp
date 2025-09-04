using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [Header("���� ����")]
    public float attackSpeed = 2.5f;       // ���� �ִϸ��̼� �ӵ�
    public float attackPower = 20f;        // ���ݷ�
    public float attackRange = 1f;         // ���� ���� ����
    public float raycastYOffset = 1f;      // ���� ���� ���� (ĸ�� �ݶ��̴� �߽� ����)
    public LayerMask enemyLayer;           // ���� ������ ���̾� (Enemy)

    [Header("��Ÿ��")]
    public float attackCooldown = 0.5f;    // ���� �� �ּ� �ð�
    private float lastAttackTime = 0f;     // ������ ���� ���� ���

    private bool isAutoAttacking = false;  // �ڵ� ���� ����
    private EnemyStats targetEnemy;        // ���� ���� ���
    private HealthBarUI targetHealthBar;   // ��� ü�¹� UI
    private Animation animationComponent;  // Animator ��� Unity Legacy Animation ���

    // ���� Ÿ�� ��ȯ
    public EnemyStats GetCurrentTarget() => targetEnemy;

    // Ÿ�� ���� �� �ڵ� ���� ����
    public void SetTarget(EnemyStats enemy)
    {
        targetEnemy = enemy;
        targetHealthBar = enemy?.GetComponentInChildren<HealthBarUI>();
        if (enemy != null)
        {
            isAutoAttacking = true;
            lastAttackTime = 0f; // ��� ���� ����
        }
    }

    // �ʱ�ȭ
    void Awake()
    {
        animationComponent = GetComponent<Animation>();
        if (animationComponent == null)
            Debug.LogError("Animation ������Ʈ�� Player ������ �Ǵ� �ڽĿ� �����ϴ�!");
    }

    // �� ������ ������Ʈ
    void Update()
    {
        // ���콺 ������ Ŭ��: Ÿ�� ����
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                EnemyStats clickedEnemy = hit.collider.GetComponent<EnemyStats>();

                // Ŭ���� ����� Enemy�̰� enemyLayer�� ���ϸ� Ÿ�� ����
                if (clickedEnemy != null && ((1 << hit.collider.gameObject.layer) & enemyLayer) != 0)
                {
                    SetTarget(clickedEnemy); // �ڵ� ���� ����
                }
                else
                {
                    // �� ���� Ŭ�� �� Ÿ�� ����
                    isAutoAttacking = false;
                    targetEnemy = null;
                    targetHealthBar = null;

                    // ���� ���̸� �⺻ ��� �ִϸ��̼� ���
                    if (animationComponent != null)
                        animationComponent.Play("Stand (ID 0 variation 0)");
                }
            }
        }

        // �ڵ� ���� ����
        if (isAutoAttacking && targetEnemy != null && targetEnemy.currentHP > 0)
        {
            // ���� ��Ÿ�� Ȯ��
            if (Time.time >= lastAttackTime)
            {
                // ���� ���� üũ (Collider ��� �Ÿ� ���)
                Collider enemyCollider = targetEnemy.GetComponent<Collider>();
                Vector3 playerOrigin = transform.position + Vector3.up * raycastYOffset;
                Vector3 closest = enemyCollider.ClosestPoint(playerOrigin);
                float distance = Vector3.Distance(playerOrigin, closest);

                if (distance <= attackRange)
                {
                    PerformAttack();                     // ���� ����
                    lastAttackTime = Time.time + attackCooldown; // ��Ÿ�� ����
                }
            }

            // Ÿ�� �������� ȸ��
            RotateTowardsTarget(targetEnemy.transform.position);
        }
        else if (targetEnemy == null || targetEnemy.currentHP <= 0)
        {
            // Ÿ���� ���ų� ������ �ڵ� ���� ����
            isAutoAttacking = false;
            targetEnemy = null;
            targetHealthBar = null;

            // ���� �ִϸ��̼��� ��� ���̸� �⺻ ��� �ִϸ��̼����� ����
            if (animationComponent != null && animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                animationComponent.Play("Stand (ID 0 variation 0)");
        }
    }

    // Ÿ�� �������� �ε巴�� ȸ��
    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // ���� ȸ�� ����
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    // ���� ����
    void PerformAttack()
    {
        if (targetEnemy == null)
        {
            Debug.LogWarning("TargetEnemy is null!");
            return;
        }

        string animName = "Attack1H (ID 17 variation 0)";

        // ���� �ִϸ��̼� ���
        if (animationComponent.GetClip(animName) != null)
        {
            animationComponent[animName].speed = attackSpeed;
            animationComponent.Play(animName); // �ִϸ��̼� ���
        }
        else
        {
            Debug.LogError($"�ִϸ��̼� {animName}�� ã�� �� �����ϴ�!");
        }

        // ���� ��/�� HP �α� ���
        Debug.Log($"Before Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");
        targetEnemy.TakeDamage(attackPower); // ���� ������ ����
        Debug.Log($"After Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");

        // ü�¹� ����
        if (targetHealthBar != null)
            targetHealthBar.CheckHp();
    }
}