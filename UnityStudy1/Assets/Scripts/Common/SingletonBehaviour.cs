using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T> // �̱��ϵ��� ���̽� Ŭ����
{
    protected bool m_IsDestroyOnLoad = false; // DestroyOnLoad ����

    // �� Ŭ������ ����ƽ �ν��Ͻ� ����
    protected static T m_Instance;

    public static T Instance
    {
        get { return m_Instance; }
    }

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (m_Instance == null)
        {
            m_Instance = (T)this;

            if (m_IsDestroyOnLoad == false)
            {
                DontDestroyOnLoad(this);
            }
        }
        else
        {
            Destroy(gameObject); // �̹� �̱����� ������������� �ڻ�
        }
    }

    // ��ü�� �μ����� ȣ���
    protected virtual void OnDestroy()
    {
        Dispose();
    }

    // ��ü �μ����� ó���� �۾�
    protected virtual void Dispose()
    {
        m_Instance = null;
    }
}
