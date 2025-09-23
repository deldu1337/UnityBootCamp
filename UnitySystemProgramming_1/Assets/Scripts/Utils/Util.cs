using UnityEngine;
public class Util
{
    public static T GetorAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }

        return component;
    }

    // 부모 오브젝트와 찾으려는 자식오브젝 이름을 받아서 재귀적, 또는 비재귀적 으로 탐색 후 게임 오브젝트를 반환
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    // 부모 오브젝트와 찾으려는 자식오브젝트 이름을 받아서 재귀적, 또는 비재귀적 으로 탐색 후 찾으려는 컴퍼넌트 반환
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : Object
    {
        // 부모 오브젝트가 널 이면 널 반환
        if (go == null)
            return null;

        // 재귀적으로 찾을지 여부
        if (recursive == false) // 직속 자식만 찾는 경우
        {
            // 부모가 들고 있는 자식 수 만큼 반복
            for (int i = 0; i < go.transform.childCount; i++)
            {
                // 0번째 자식부터 하나씩 트랜스폼을 가져옴
                Transform transform = go.transform.GetChild(i);
                // 가져온 트랜스폼을 대상으로 이름을 비교할건데
                // 이름이 비어있으면 제일 처음에 찾은 컴퍼넌트를 바로 반환할 것, 
                // 이름이 있다면 해당 이름을 갖고있는 오브젝트의 컴퍼넌트를 반환
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    // 0번째 자식부터 자식의 컴퍼넌트를 가져옴
                    T component = transform.GetComponent<T>();
                    // 컴퍼넌트가 null 이 아니면 해당 컴퍼넌트 반환
                    if (component != null)
                        return component;
                }
            }
        }
        else // 재귀적으로 자식의 자식 까지 모두 찾는 경우
        {
            // 자식의 자식까지 모조리 전부다 꺼내서 하나씩 전체 반복
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                // 가져온 컴퍼넌트를 대상으로 이름을 비교할건데
                // 이름이 비어있으면 제일 처음에 찾은 컴퍼넌트를 바로 반환할 것, 
                // 이름이 있다면 해당 이름을 갖고있는 오브젝트의 컴퍼넌트를 반환
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }
}
