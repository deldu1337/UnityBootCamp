using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public class DamagedObject : MonoBehaviour
{
    public Color damaged;
    public Color origin;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnDamaged()
    {
        StopAllCoroutines(); //���� �ڷ�ƾ ����
        StartCoroutine(SetColor(damaged)); //�� ����
    }

    private IEnumerator SetColor(Color cl)
    {
        spriteRenderer.color = cl;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = origin;
    }

}
