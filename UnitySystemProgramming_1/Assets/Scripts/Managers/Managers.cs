using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers s_instance; // 유일성이 보장됨
    public static Managers Instance { get { Init(); return s_instance; }  } // 유일한 매니저를 갖고옴

    private InputManager _input = new InputManager();
    private ResouceManager _resouce = new ResouceManager();
    private UIManager _ui = new UIManager();
    public static InputManager Input { get { return Instance._input; } }
    public static ResouceManager Resource { get { return Instance._resouce; } }
    public static UIManager UI { get { return Instance._ui; } }

    void Start()
    {
        Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
        }
    }
}
