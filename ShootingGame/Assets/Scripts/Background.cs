using UnityEngine;

public class Background : MonoBehaviour
{
    public Material material;

    public float scrollSpeed = 0.2f;

    private void Update()
    {
        Vector2 dir = Vector2.up;
        material.mainTextureOffset += dir * scrollSpeed * Time.deltaTime;
    }
}
