using UnityEngine;

// 수정사항이 바로 반영되고 관리하기 편하여 스크립터블 오브젝트 사용
[CreateAssetMenu(fileName = "최고점수", menuName = "best/최고점수")]
public class BestScore : ScriptableObject
{
    public int best;
}
