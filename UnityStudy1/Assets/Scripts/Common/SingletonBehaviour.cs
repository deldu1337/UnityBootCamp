using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T> // 싱글턴들의 베이스 클래스
{
    protected bool m_IsDestroyOnLoad = false; // DestroyOnLoad 여부

    // 이 클래스의 스태틱 인스턴스 변수
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
            Destroy(gameObject); // 이미 싱글턴이 만들어져있으면 자살
        }
    }

    // 객체가 부서질때 호출됨
    protected virtual void OnDestroy()
    {
        Dispose();
    }

    // 객체 부서질떄 처리할 작업
    protected virtual void Dispose()
    {
        m_Instance = null;
    }
}
