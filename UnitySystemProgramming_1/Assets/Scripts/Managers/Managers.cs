using System.Resources;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers s_instance;
    public static Managers Instance { get { Init(); return s_instance; } }

    private InputManager _input = new InputManager();
    private ResouceManager _resouce = new ResouceManager();
    public static InputManager Input { get { return Instance._input; } }
    public static ResouceManager Resource { get { return Instance._resouce; } }


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