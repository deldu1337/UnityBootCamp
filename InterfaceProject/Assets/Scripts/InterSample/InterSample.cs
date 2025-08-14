using UnityEngine;

// 인터페이스(interface)
// 앞에 대문자 I (인터페이스라는 걸 알려주기 위함, 암묵적 룰)

public interface IThrowable
{
    
}

public interface IWeapon
{

}

public interface ICountable
{

}

public interface IPotion
{

}

public interface IUsable
{

}

public class Item
{

}

public class Sword : Item, IWeapon { }
public class Jabelin : Item, IWeapon, ICountable, IThrowable { }

public class MaxPotion : Item, IPotion, ICountable, IUsable { }

public class FirePotion : Item, IPotion, ICountable, IThrowable { }

public class InterSample : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
