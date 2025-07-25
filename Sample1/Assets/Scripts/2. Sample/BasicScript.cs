using A;
using UnityEngine;
// 1. 유니티 엔진 관련 기능을 사용하겠습니다.
// 해당 코드 내에서 사용되는 특정 기능들은 UnityEngine 네임스페이스의 기능으로 해석됩니다.

// 네임 스페이스는 특정 데이터를 묶어주는 용도로 사용할 수 있습니다.(이름 충돌 문제를 해결합니다.)
// 분류를 하는 것이 목적입니다.
namespace A
{
    public class Item
    {
        public int id;
    }
}

namespace B
{
    public class Item
    {
        public int id2;
    }
}

// 네임 스페이스 사용 방법
// 1. 네임 스페이스.클래스명과 같이 네임 스페이스를 직접 작성해서 사용하는 경우

// 2. using을 이용해 해당 코드 내에서 사용하는 값은 그 네임 스페이스의 값임을 알리고 사용하는 경우


// 2. 클래스의 이름은 BasicScript
// 이 스크립트는 MonoBehaviour의 기능을 포함하고 있습니다. (C#의 상속)
// 스크립트의 이름과 같은 class가 반드시 존재해야 합니다.

// MonoBehaviour 클래스는 에디터에서 게임 오브젝트에 스크립트를 컴포넌트로써 연결할 수 있는 프레임워크를 제공합니다.
// Start, Update와 같은 이벤트에 대한 연결도 제공합니다.

public class BasicScript : MonoBehaviour
{
// 3. 유니티 생명 주기(이벤트)
// 유니티에서는 사전에 지정한 순서대로 여러 개의 이벤트 함수가 실행이 됩니다.
// 사용자는 해당 이벤트 함수에 기능을 작성하는 것으로 원하는 상황에 맞는 작업을 처리할 수 있습니다.
    void Start()
    {
        Item item = new Item(); // 위에 있는 using A에 의해 이 Item은 자동으로 A의 Item을 의미하게 됩니다.
        B.Item item2 = new B.Item(); // 네임 스페이스를 작성해줌으로써, 해당 위치의 Item임을 표현합니다.
    }

    void Update()
    {
        
    }
}
