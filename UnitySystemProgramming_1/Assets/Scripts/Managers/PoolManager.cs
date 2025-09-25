using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PoolManager
{
    // @Pool_Root (PoolManager)
    //      ㄴ> UnityChan_Root (Pool)
    //           ㄴ> UnityChan (Poolable)       
    //           ㄴ> UnityChan (Poolable)
    //           ㄴ> UnityChan (Poolable)
    //           ㄴ> UnityChan (Poolable)
    //           ㄴ> UnityChan (Poolable)
    //      ㄴ> Monster_Root (Pool)
    //          ㄴ> Monster (Poolable) // 비활성화
    //          ㄴ> Monster (Poolable) // 비활성화
    //          ㄴ> Monster (Poolable) // 비활성화
    //          ㄴ> Monster (Poolable) // 비활성화
    #region Pool
    class Pool
    {
        public GameObject Original { get; private set; } // 프리펩을 담고 있는 프로퍼티
        public Transform Root; // 부모의 개념을 위한 변수

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject original, int count = 5) // 풀을 처음에 셋팅하는 함수(풀러블 객체 미리 몇개 생성)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Root";

            for (int i = 0; i < count; i++)
            {
                Push(Create());
            }
        }

        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original); // 새로운 게임 오브젝트 생성
            go.name = Original.name; // 오브젝트의 이름을 변경
            return go.GetorAddComponent<Poolable>();
        }

        public void Push(Poolable poolable) // _poolStack 에 붙이고 하이어라키도 정리 해주는 함수(즉, 풀에 반납하는 함수)
        {
            if (poolable == null)
                return;

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.IsUsing = false;

            _poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent)
        {
            Poolable poolable;

            if (_poolStack.Count > 0) // 풀에 있으면 그냥 넘겨주기
                poolable = _poolStack.Pop();
            else // 현재 풀에 아무것도 없는경우 새로 풀을 생성해서 초기화 후 넘겨줌
                poolable = Create();

            poolable.gameObject.SetActive(true);

            // DontDestroyOnLoad 해제
            if (parent == null)
                poolable.transform.parent = Managers.Scene.CurrentScene.transform;

            poolable.transform.parent = parent; // parent 가 널이라면 그냥 하이어라키 최상단에 붙게됨
            poolable.IsUsing = true;

            return poolable;
        }
    }
    #endregion
    
    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    Transform _root;

    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    public void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original);
        pool.Root.parent = _root;

        _pool.Add(original.name, pool);
    }

    public void Push(Poolable poolable) // 풀에서 꺼내쓰고 다 사용 하고 다시 집어 넣기
    {
        string name = poolable.gameObject.name;

        if (_pool.ContainsKey(name) == false)
        {
            Managers.Resource.Destroy(poolable.gameObject);
            return;
        }

        _pool[name].Push(poolable);
    }

    public Poolable Pop(GameObject original, Transform parent = null) // 사용 하기 위해 꺼내기
    {
        // 만약 풀자체가 없다면 풀자체도 만들어주기
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original);

        return _pool[original.name].Pop(parent);
    }

    public GameObject GetOtiginal(string name) // 프리펩을 가져오는 함수
    {
        if (_pool.ContainsKey(name) == false)
            return null;

        return _pool[name].Original;
    }

    public void Clear()
    {
        foreach (Transform child in _root)
            Managers.Resource.Destroy(child.gameObject); // 하이어라키상 에서 삭제

        _pool.Clear(); // 저장공간에서도 비우기
    }
}

//public class PoolManager
//{
//    #region Pool
//    class Pool // 이너 클래스
//    {
//        public GameObject Original { get; private set; } // 원본 프리펩
//        public Transform Root { get; private set; } // ex) UnityChan_Root

//        Stack<Poolable> _poolStack = new Stack<Poolable>(); // 실제 풀러블 객체를 담을 저장소

//        public void Init(GameObject original, int count = 5)
//        {
//            Original = original;
//            Root = new GameObject().transform;
//            Root.name = $"{original.name}_Root"; // <- UnityChan_Root

//            for (int i = 0; i < count; i++)
//            {
//                Push(Create());
//            }
//        }

//        Poolable Create()
//        {
//            GameObject go = Object.Instantiate<GameObject>(Original);
//            go.name = Original.name;
//            return go.GetorAddComponent<Poolable>();
//        }

//        public void Push(Poolable poolable)
//        {
//            if (poolable == null)
//                return;

//            poolable.transform.parent = Root;
//            poolable.gameObject.SetActive(false);
//            poolable.IsUsing = false;

//            _poolStack.Push(poolable);
//        }

//        public Poolable Pop(Transform parent)
//        {
//            Poolable poolable;

//            if (_poolStack.Count > 0)
//                poolable = _poolStack.Pop();
//            else
//                poolable = Create();

//            poolable.gameObject.SetActive(true);

//            if (parent == null)
//                poolable.transform.parent = Managers.Scene.CurrentScene.transform;

//            poolable.transform.parent = parent;
//            poolable.IsUsing = true;

//            return poolable;
//        }
//    }
//    #endregion

//    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
//    // @Pool (PoolManager)
//    //  ㄴ> UnityChan_Root (Pool)
//    //       ㄴ> UnityChan (Poolable)       
//    //       ㄴ> UnityChan (Poolable)
//    //       ㄴ> UnityChan (Poolable)
//    //       ㄴ> UnityChan (Poolable)
//    //       ㄴ> UnityChan (Poolable)
//    //  ㄴ> Monster_Root (Pool)
//    //       ㄴ> Monster (Poolable)
//    //       ㄴ> Monster (Poolable)
//    //       ㄴ> Monster (Poolable)
//    //       ㄴ> Monster (Poolable)

//    Transform _root;

//    public void Init()
//    {
//        if (_root == null)
//        {
//            _root = new GameObject { name = "@Pool_Root" }.transform;
//            Object.DontDestroyOnLoad(_root);
//        }
//    }

//    public void CreatePool(GameObject original, int count = 5)
//    {
//        Pool pool = new Pool();
//        pool.Init(original, count);
//        pool.Root.parent = _root;

//        _pool.Add(original.name, pool);
//    }

//    public void Push(Poolable poolable) // 다 사용 하고 다시 집어 넣기
//    {
//        string name = poolable.gameObject.name;
//        if (_pool.ContainsKey(name) == false) // 우리가 관리하는 풀에 해당하는 키값이 있는지 확인
//        {
//            GameObject.Destroy(poolable.gameObject); // 풀로 만든애가 아니라면 삭제 처리
//            return;
//        }

//        _pool[name].Push(poolable);
//    }

//    public Poolable Pop(GameObject original, Transform parent = null) // 사용 하기 위해 꺼내기
//    {
//        if (_pool.ContainsKey(original.name) == false)
//            CreatePool(original);

//        return _pool[original.name].Pop(parent);
//    }

//    public GameObject GetOriginal(string name)
//    {
//        if (_pool.ContainsKey(name) == false)
//            return null;

//        return _pool[name].Original;
//    }

//    public void Clear()
//    {
//        foreach(Transform child in _root)
//            Managers.Resource.Destroy(child.gameObject);

//        _pool.Clear();
//    }
//}
