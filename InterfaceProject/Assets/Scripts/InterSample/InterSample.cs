using UnityEngine;

// �������̽�(interface)
// �տ� �빮�� I (�������̽���� �� �˷��ֱ� ����, �Ϲ��� ��)

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
