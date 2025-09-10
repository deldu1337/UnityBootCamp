using System;
using System.Collections.Generic;
using UnityEngine;


class PriorityQueue<T> where T : IComparable<T> // <= 얘를 상속받아 구현한 클래스만 T 가능
{
    List<T> _heap = new List<T>();

    // 삽입
    public void Push(T data)
    {
        _heap.Add(data);
        // 일단 노드 맨 아래 추가

        int now = _heap.Count - 1;
        while (now > 0)
        {
            // 부모 구하기
            int next = (now - 1) / 2;

            // 내가 부모보다 작다면 브레이크
            if (_heap[now].CompareTo(_heap[next]) < 0)
                break;

            // 부모랑 나의 위치를 교환
            T temp = _heap[now];
            _heap[now] = _heap[next];
            _heap[next] = temp;

            now = next;
        }
    }

    public T Pop()
    {
        // 반환 데이터 저장
        T ret = _heap[0];

        // 마지막 인덱스 가져오기
        int lastIndex = _heap.Count - 1;
        // 루트 노드의 데이터를 마지막 데이터와 교체하기
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);
        lastIndex--;


        int now = 0;
        while (true)
        {
            // 왼쪽 자식 노드 구하기
            int left = 2 * now + 1;

            // 오른쪽 자식 노드 구하기
            int right = 2 * now + 2;

            int next = now;
            if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                next = left;

            if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                next = right;

            // 만약, 왼쪽 오른쪽 모두 now 보다 작다면 종료
            if (next == now)
                break;

            // 이제 두 값을 교체
            T temp = _heap[now];
            _heap[now] = _heap[next];
            _heap[next] = temp;
            now = next;
        }

        return ret;
    }

    public int Count { get { return _heap.Count; } }
}

