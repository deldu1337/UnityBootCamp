using System;
using System.Collections.Generic;
using UnityEngine;


class PriorityQueue<T> where T : IComparable<T> // <= �긦 ��ӹ޾� ������ Ŭ������ T ����
{
    List<T> _heap = new List<T>();

    // ����
    public void Push(T data)
    {
        _heap.Add(data);
        // �ϴ� ��� �� �Ʒ� �߰�

        int now = _heap.Count - 1;
        while (now > 0)
        {
            // �θ� ���ϱ�
            int next = (now - 1) / 2;

            // ���� �θ𺸴� �۴ٸ� �극��ũ
            if (_heap[now].CompareTo(_heap[next]) < 0)
                break;

            // �θ�� ���� ��ġ�� ��ȯ
            T temp = _heap[now];
            _heap[now] = _heap[next];
            _heap[next] = temp;

            now = next;
        }
    }

    public T Pop()
    {
        // ��ȯ ������ ����
        T ret = _heap[0];

        // ������ �ε��� ��������
        int lastIndex = _heap.Count - 1;
        // ��Ʈ ����� �����͸� ������ �����Ϳ� ��ü�ϱ�
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);
        lastIndex--;


        int now = 0;
        while (true)
        {
            // ���� �ڽ� ��� ���ϱ�
            int left = 2 * now + 1;

            // ������ �ڽ� ��� ���ϱ�
            int right = 2 * now + 2;

            int next = now;
            if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                next = left;

            if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                next = right;

            // ����, ���� ������ ��� now ���� �۴ٸ� ����
            if (next == now)
                break;

            // ���� �� ���� ��ü
            T temp = _heap[now];
            _heap[now] = _heap[next];
            _heap[next] = temp;
            now = next;
        }

        return ret;
    }

    public int Count { get { return _heap.Count; } }
}

