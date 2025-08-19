using System.Threading;
using UnityEngine;
// ��ǥ: ���� �ð����� ���� ������ �� ��ġ�� ���� ��.
// �ʿ��� ������: ���� �ð�, ���� �ð�, �� ���� ����
// �۾� ����
// 1. �ð��� üũ�ϰ�
// 2. ���� �ð��� ���� �ð��� �ȴٸ�(�� Ÿ��, �� Ÿ��, ...)
// 3. ���� �����մϴ�.

public class EnemyManager : MonoBehaviour
{
    float min = 1, max = 5; // ��ȯ �ð� ����(�ִ� �ּ�) ���� �ֱ� ����

    float currentTime;
    public float createTime = 1.0f;
    public GameObject enemyFactory;
    public GameObject spawnArea; // ���� ����

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > createTime)
        {
            if (StageManager.count < 20 && StageManager.count != -1)
            {
                var enemy = Instantiate(enemyFactory, spawnArea.transform.position, Quaternion.identity);
                // ������ �Ŵ��� ��ȯ ����(spawn area)�� ������ ������ ������,
                // ���� ��ġ�� ȸ�� �� ���� ���� �ʾƵ� �ȴ�.
                // ������ ���� �����Ǿ� �ִٸ� ���� ��ġ�� �����Ѵ�.

                currentTime = 0; // ������ �ð��� ������, �ٽ� ���ǹ��� üũ�� �� �ֵ��� �����մϴ�.
                createTime = Random.Range(min, max);
            }
        }
    }
}
