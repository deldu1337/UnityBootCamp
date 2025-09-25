using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    // 로고를 위한 변수들
    public Animation _logoAnim;
    public TextMeshProUGUI _logoText;

    // 타이틀을 위한 변수들
    public GameObject _title;
    public Slider _loadingSlider;
    public TextMeshProUGUI _loadingProgressText;

    // 비동기 현재 상태를 받기 위한 변수
    private AsyncOperation _asyncOperation;

    private void Awake()
    {
        _logoAnim.gameObject.SetActive(true);
        _title.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(LoadGameCoroutine());
    }

    private IEnumerator LoadGameCoroutine()
    {
        Logger.Log($"{GetType()}::LoadGameCoroutine");

        _logoAnim.Play();
        yield return new WaitForSeconds(_logoAnim.clip.length);

        _logoAnim.gameObject.SetActive(false);
        _title.SetActive(true);

        _asyncOperation = SceneLoader.Instance.LoadSceneAsync(SceneType.Lobby);
        if (_asyncOperation == null)
        {
            Logger.LogError("Lobby async loading error.");
            yield break;
        }

        _asyncOperation.allowSceneActivation = false; // 0.1~0.9 -> 씬이동을 자동으로 함, 그걸 막기 위한거


        //_loadingSlider.value = 0.5f;
        _loadingProgressText.text = ((int)(_loadingSlider.value * 100)).ToString();
        yield return new WaitForSeconds(0.5f);

        while (_asyncOperation.isDone == false)
        {
            _loadingSlider.value = _asyncOperation.progress;
            _loadingProgressText.text = ((int)(_loadingSlider.value * 100)).ToString();
            
            // 씬 로딩이 완료 되었다면 코루틴 끄고 이동하기
            if (_asyncOperation.progress >= 0.9f)
            {
                _asyncOperation.allowSceneActivation = true;
                yield break;
            }

            yield return null;
        }
        
    }
}
