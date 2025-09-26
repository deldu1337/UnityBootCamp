using UnityEngine;

public class RandomMap : MonoBehaviour
{
    [SerializeField] private GameObject[] RoomPrefab;
    [SerializeField] private Transform SpawningRoom;

    void Start()
    {
        ChangeRoom();
    }

    public void ChangeRoom()
    {
        if (SpawningRoom.childCount > 0)
            Destroy(SpawningRoom.GetChild(0).gameObject);

        int random = Random.Range(0, RoomPrefab.Length);
        int[] arrRotation = { 0, 90, 180, 270 };
        int randomRotation = Random.Range(0, 4);
        Random.InitState(randomRotation);
        Vector3 vector3 = Vector3.zero;
        //Quaternion rotation = new Quaternion(0, arrRotation[randomRotation], 0, 0);
        Quaternion rotation = new Quaternion(0, 90, 0, 0);
        Instantiate(RoomPrefab[random], vector3, rotation, SpawningRoom);
    }
}
