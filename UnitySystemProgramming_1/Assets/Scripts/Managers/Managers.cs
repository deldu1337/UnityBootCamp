using System;
using UnityEditor.PackageManager;
using UnityEngine;

// ΩÃ±€≈Ê ∆–≈œ
public class Managers : MonoBehaviour
{
    static Managers s_instance; // ¿Ø¿œº∫¿Ã ∫∏¿Âµ 
    static public Managers Instance { get { Init();  return s_instance; } }

    private InputManager _input = new InputManager();
    private ResourceManager _resource = new ResourceManager();
    public static InputManager Input { get { return Instance._input; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
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
        if(s_instance == null)
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
