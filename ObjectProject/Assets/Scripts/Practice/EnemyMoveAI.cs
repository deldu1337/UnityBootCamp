using System.Collections;
using UnityEngine;

public class EnemyMoveAI : MonoBehaviour
{
    public float speed = 2.0f; // �̵� �ӵ�

    private Transform player_position; // �÷��̾� ��ġ
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_position = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player_position != null)
        {
            StartCoroutine(EnemyMove());
        }
        else
        {
            Debug.LogWarning("���� ������ �÷��̾ ã�� �� �����ϴ�.");
        }
    }

    IEnumerator EnemyMove()
    {
        while (player_position != null)
        {
            float distance = Vector3.Distance(transform.position, player_position.position);

            transform.position = Vector3.MoveTowards(transform.position, player_position.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(transform.position);

            yield return null;
        }
    }
}
