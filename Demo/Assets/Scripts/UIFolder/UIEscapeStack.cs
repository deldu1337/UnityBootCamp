using System;
using System.Collections.Generic;
using UnityEngine;

public class UIEscapeStack : MonoBehaviour
{
    public static UIEscapeStack Instance { get; private set; }

    private class Entry
    {
        public string key;
        public Action close;
        public Func<bool> isOpen;
    }

    private readonly List<Entry> _stack = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>������ �����. ��𼭵� Instance �����.</summary>
    public static UIEscapeStack GetOrCreate()
    {
        if (Instance != null) return Instance;
        var go = new GameObject("UIEscapeStack");
        return go.AddComponent<UIEscapeStack>();
    }

    /// <summary>UI�� �� �� ȣ��: ESC�� ���� �׼��� ���ÿ� ���</summary>
    public void Push(string key, Action close, Func<bool> isOpen = null)
    {
        if (string.IsNullOrEmpty(key) || close == null) return;
        Remove(key); // �ߺ� ����
        _stack.Add(new Entry { key = key, close = close, isOpen = isOpen });
    }

    /// <summary>UI�� ���� �� ȣ��: ���ÿ��� ����</summary>
    public void Remove(string key)
    {
        if (string.IsNullOrEmpty(key)) return;
        _stack.RemoveAll(e => e.key == key);
    }

    /// <summary>���� �ֱ� �׸��� �ݴ´�. ���� ���� ������ false.</summary>
    public bool PopTop()
    {
        // �ڿ������� ��ĵ (���� �ֱ�)
        for (int i = _stack.Count - 1; i >= 0; i--)
        {
            var e = _stack[i];
            // �̹� ���� �ִٸ� ������ ����
            if (e.isOpen != null && !e.isOpen())
            {
                _stack.RemoveAt(i);
                continue;
            }
            _stack.RemoveAt(i);
            try { e.close?.Invoke(); } catch (Exception ex) { Debug.LogException(ex); }
            return true;
        }
        return false;
    }

    public bool IsEmpty => _stack.Count == 0;
}
