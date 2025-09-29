using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private Animation _logoAnim;
    [SerializeField] private TextMeshProUGUI _logoText;

    [SerializeField] private GameObject _title;
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private TextMeshProUGUI _loadingProgressText;

    private AsyncOperation m_asyncOperation;

    private void Awake()
    {
        _logoAnim.gameObject.SetActive(true);
        _title.SetActive(false);
    }

    private void Start()
    {
        // 유저 데이터 로드
        UserDataManager.Instance.LoadUserData();

        // 저장된 유저 데이터가 없으면 기본값으로 세팅 후 저장
        if (UserDataManager.Instance.ExistsSaveData == false)
        {
            UserDataManager.Instance.SetDefaultUserData();
            UserDataManager.Instance.SaveUserData();
        }

        // temp
        ConfirmUIData confirmUIData = new ConfirmUIData();
        confirmUIData.ConfirmType = ConfirmType.OK;
        confirmUIData.TitleText = "UI TEST";
        confirmUIData.DescText = "THIS IS UI TEST";
        confirmUIData.OKBtnText = "OK";
        UIManager.Instance.OpenUI<ConfirmUI>(confirmUIData);


        //StartCoroutine(LoadGameCo());
    }

    private IEnumerator LoadGameCo()
    {
        Logger.Log($"{GetType()}::LoadGameCo");

        _logoAnim.Play();
        yield return new WaitForSeconds(_logoAnim.clip.length);

        _logoAnim.gameObject.SetActive(false);
        _title.SetActive(true);

        m_asyncOperation = SceneLoader.Instance.LoadSceneAsync(SceneType.Lobby);
        if (m_asyncOperation == null)
        {
            Logger.LogError("Lobby async loading error.");
            yield break;
        }

        m_asyncOperation.allowSceneActivation = false;

        _loadingSlider.value = 0.5f;
        _loadingProgressText.text = $"{(int)(_loadingSlider.value * 100)} %";
        yield return new WaitForSeconds(0.5f);

        while(m_asyncOperation.isDone == false)
        {
            _loadingSlider.value = m_asyncOperation.progress < 0.5f ? 0.5f : m_asyncOperation.progress;
            _loadingProgressText.text = $"{(int)(_loadingSlider.value * 100)} %";

            if (m_asyncOperation.progress >= 0.9f)
            {
                m_asyncOperation.allowSceneActivation = true;
                yield break;
            }

            yield return null;
        }
    }
}
