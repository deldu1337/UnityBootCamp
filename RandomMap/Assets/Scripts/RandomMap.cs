using UnityEngine;

public class RandomMap : MonoBehaviour
{
    [SerializeField] private GameObject[] RoomPrefab;
    [SerializeField] private Transform SpawningRoom;

    void Start()
    {
        
    }

    public void ChangeRoom()
    {
        if (SpawningRoom.childCount > 0)
            Destroy(SpawningRoom.GetChild(0).gameObject);

        int random = Random.Range(0, RoomPrefab.Length);
        int[] arrRotation = { 0, 90, 180, 270 };
        int randomRotation = Random.Range(0, 4);
        Vector3 vector3 = Vector3.zero;
        Quaternion rot = Quaternion.Euler(0f, arrRotation[randomRotation], 0f);
        Instantiate(RoomPrefab[random], vector3, rot, SpawningRoom);
    }

    public void EnterRoom()
    {

    }
}
