using UnityEngine;

// 에디터에서 해당 오브젝트 생성 가능
[CreateAssetMenu(fileName = "정답", menuName = "Answer/정답")]
public class AnswerSample : ScriptableObject
{
    public string[] quiz;
    public string[] answer;
}
