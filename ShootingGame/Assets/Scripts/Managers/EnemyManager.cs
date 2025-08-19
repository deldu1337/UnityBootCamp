using System.Threading;
using UnityEngine;
// 목표: 일정 시간마다 적을 생성해 내 위치에 놓을 것.
// 필요한 데이터: 일정 시간, 현재 시간, 적 생성 공장
// 작업 순서
// 1. 시간을 체크하고
// 2. 현재 시간이 일정 시간이 된다면(젠 타임, 쿨 타임, ...)
// 3. 적을 생성합니다.

public class EnemyManager : MonoBehaviour
{
    float min = 1, max = 5; // 소환 시간 간격(최대 최소) 생성 주기 조작

    float currentTime;
    public float createTime = 1.0f;
    public GameObject enemyFactory;
    public GameObject spawnArea; // 생성 지역

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > createTime)
        {
            if (StageManager.count < 20 && StageManager.count != -1)
            {
                var enemy = Instantiate(enemyFactory, spawnArea.transform.position, Quaternion.identity);
                // 현재의 매니저 소환 지점(spawn area)에 생성을 진행할 것으로,
                // 따로 위치나 회전 값 제공 하지 않아도 된다.
                // 지점이 따로 설정되어 있다면 지점 위치에 생성한다.

                currentTime = 0; // 현재의 시간을 리셋해, 다시 조건문을 체크할 수 있도록 설계합니다.
                createTime = Random.Range(min, max);
            }
        }
    }
}
