using UnityEngine;
public class ResouceManager
{
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject)) // 프리펩을 로드할려는 확률이 높음
        {
            string name = path;
            int index = name.LastIndexOf('/'); 
            if (index >= 0) // /Player   =>  Player
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOtiginal(name);
            if (go != null)
                return go as T;
        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        // 1. 무작정 Resources.Load 하지말고 일단 이미 이전에 Load 했는지 확인후 가져오기
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
            Debug.LogError($"{path} 프리펩 없음");

        // 2. 혹시 풀링된 오브젝트가 이미 있을까?
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;

        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        // 만약에 풀링이 필요한 아이라면 -> 풀매니저에 반납
        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}

//public class ResouceManager
//{
//    public T Load<T>(string path) where T : Object
//    {
//        if (typeof(T) == typeof(GameObject))
//        {
//            string name = path;
//            int index = name.LastIndexOf('/');
//            if (index >= 0)
//                name = name.Substring(index + 1); // /Monster/Orc 라는 주소라면 Orc 만 분리

//            GameObject go = Managers.Pool.GetOriginal(name); // Pool 에서는 해당 오리지널의 이름만 필요
//            if (go != null)
//                return go as T;
//        }

//        return Resources.Load<T>(path); // FullPath 필요
//    }

//    public GameObject Instantiate(string path, Transform parent = null)
//    {
//        GameObject original = Load<GameObject>($"Prefabs/{path}");
//        if (original == null)
//        {
//            Debug.LogError($"프리펩 없음 : {path}");
//        }

//        if (original.GetComponent<Poolable>() != null)
//            return Managers.Pool.Pop(original, parent).gameObject;

//        GameObject go = Object.Instantiate(original, parent);
//        go.name = original.name;

//        return go;
//    }

//    public void Destroy(GameObject go)
//    {
//        if (go == null)
//            return;

//        Poolable poolable = go.GetComponent<Poolable>();
//        if (poolable != null)
//        {
//            Managers.Pool.Push(poolable);
//            return;
//        }

//        Object.Destroy(go);
//    }
//}
