using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    public GameObject effect_prefab;
    public int size = 30;

    // Ǯ�� ���� ���Ǵ� �ڷᱸ��
    // 1. ����Ʈ(List) : �����͸� ���������� �����ϰ� �߰�, ������ �����ӱ� ������ ȿ����
    // 2. ť(Queue) : �����Ͱ� ���� ������� �����Ͱ� ���������� ������ �ڷᱸ��
    private List<GameObject> pool;

    private void Start()
    {
        // �Ѿ� ����
        pool = new List<GameObject>();

        for (int i = 0; i < size; i++)
        {
            var effect = Instantiate(effect_prefab);
            effect.transform.parent = transform;
            // ������ �Ѿ��� ���� ��ũ��Ʈ�� ���� ������Ʈ�� �ڽ����� �����˴ϴ�.

            effect.SetActive(false); // ��Ȱ��ȭ ����

            effect.GetComponent<Effect>().SetPool(this);

            pool.Add(effect);
            // ����Ʈ��.Add(��) : ����Ʈ�� �ش� ���� �߰��ϴ� ����
        }
    }

    public GameObject GetEffect()
    {
        // ��Ȱ��ȭ�Ǿ��ִ� �Ѿ��� ã�Ƽ� Ȱ��ȭ�մϴ�.
        foreach (var effect in pool)
        {
            // ���� â���� Ȱ��ȭ�� �ȵǾ��ִٸ� (����ϰ� ���� �ʴ´ٸ�)
            if (!effect.activeInHierarchy)
            {
                effect.SetActive(true);
                return effect;
            }
        }
        // �Ѿ��� ������ ��쿡�� ���Ӱ� ���� ����Ʈ�� ����մϴ�.
        var new_effect = Instantiate(effect_prefab);
        new_effect.transform.parent = transform;
        new_effect.GetComponent<Effect>().SetPool(this);
        pool.Add(new_effect);
        return new_effect;
    }

    public void Return(GameObject effect)
    {
        effect.SetActive(false);
    }
}
