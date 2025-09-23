using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    // 내 자식들이 갖고 있는 컴퍼넌트들을 저장할 공간 (타입별로 모아놓음)
    Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    public abstract void Init();

    // 위에 만들어놓은 저장공간에 각 형식별로 컴퍼넌트들을 등록
    protected void Bind<T>(Type type) where T : UnityEngine.Object // 유니티 오브젝트만 들어 올 수 있음
    {
        // 타입으로 넘겨받은 이넘의 이름들을 추출
        string[] names = Enum.GetNames(type);

        // 해당 타입의 이름 개수 많큼 배열 생성
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        // 딕셔너리에 타입을 키로 지정, 그리고 만든 빈배열을 일단 넣어주기
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            // 빈배열에 대해서 하나씩 컴퍼넌트들을 찾아서 넣어주기
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true);
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"failed to bind({names[i]})!!!");
        }
    }

    // 컴퍼넌트를 저장한 공간에서 컴퍼넌트 끄집어 내기
    protected T Get<T>(int index) where T : UnityEngine.Object
    {
        // 오브젝트를 받아올 빈 배열 만들기
        UnityEngine.Object[] objects = null;
        // 해당하는 타입을 키값으로 해서 저장공간에서 배열 가져오기
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        // 배열을 가져왔으면 거기서 인덱스로 접근해서 걔를 반환해주기
        return objects[index] as T;
    }

    protected Text GetText(int index) => Get<Text>(index);
    protected Button GetButton(int index) => Get<Button>(index);
    protected Image GetImage(int index) => Get<Image>(index);
    protected GameObject GetGameObject(int index) => Get<GameObject>(index);

    public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler evt = Util.GetorAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler += action;
                break;
        }
    }
}
