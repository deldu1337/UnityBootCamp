using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSkeletonSnapshot
{
    private struct Bone
    {
        public Transform t;
        public Vector3 lp;
        public Quaternion lr;
        public Vector3 ls;
        public bool active;
    }

    private readonly List<Bone> bones = new();
    private readonly Transform rootCaptured;         // � ��Ʈ�� �������� ĸó�ߴ���
    private readonly bool includeRootLocalTransform; // ��Ʈ�� local ��ȯ�� ��������

    private readonly Vector3 worldRootPos;           // ĸó �� ��Ʈ�� ���� ��ġ(�ʿ� �� ���)
    private readonly Quaternion worldRootRot;        // ĸó �� ��Ʈ�� ���� ȸ��

    private PlayerSkeletonSnapshot(Transform root, bool includeRootLocal)
    {
        rootCaptured = root;
        includeRootLocalTransform = includeRootLocal;

        worldRootPos = root.position;
        worldRootRot = root.rotation;

        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            bones.Add(new Bone
            {
                t = t,
                lp = t.localPosition,
                lr = t.localRotation,
                ls = t.localScale,
                active = t.gameObject.activeSelf
            });
        }
    }

    /// <summary>Ư�� ��Ʈ(poseRoot) ���� Ʈ�������� ��� ������.</summary>
    public static PlayerSkeletonSnapshot Capture(Transform poseRoot, bool includeRootLocalTransform = true)
        => new PlayerSkeletonSnapshot(poseRoot, includeRootLocalTransform);

    /// <summary>
    /// ����� ���� Ʈ�������� ����. 
    /// worldPos/worldRot�� null�� �ָ� ���� ��ġ/ȸ���� �����ϰ�, ���� �ָ� �� ������ ����.
    /// </summary>
    public void Apply(Transform poseRoot, Vector3? worldPosOverride = null, Quaternion? worldRotOverride = null)
    {
        if (poseRoot != rootCaptured)
        {
            // �ٸ� ��Ʈ�� �����ص� ������ �����ϸ� ������ ���� Ȯ���� ����.
            // ������ Transform ���۷����� ���� �����ϹǷ� poseRoot �Ű������� ���� ���� ��ġ/ȸ�� �����.
        }

        // 1) (����) ��Ʈ ���� ��ȯ���� ����
        if (worldPosOverride.HasValue || worldRotOverride.HasValue)
        {
            poseRoot.SetPositionAndRotation(
                worldPosOverride ?? poseRoot.position,
                worldRotOverride ?? poseRoot.rotation
            );
        }

        // 2) ��Ȱ��/Ȱ�� ���� ���� ���� (�θ�-�ڽ� ������ �ּ�ȭ�� ���� �ڽĺ��� Ȱ���ص� OK)
        for (int i = 0; i < bones.Count; i++)
        {
            var b = bones[i];
            if (!b.t) continue;
            if (b.t.gameObject.activeSelf != b.active)
                b.t.gameObject.SetActive(b.active);
        }

        // 3) ���� Ʈ������ ����
        for (int i = 0; i < bones.Count; i++)
        {
            var b = bones[i];
            if (!b.t) continue;

            // ��Ʈ�� local ��ȯ�� �ǵ帮�� ���� ������(���� ���� ��ġ/ȸ���� ���� ���ϴ� ���) skip
            if (!includeRootLocalTransform && b.t == rootCaptured) continue;

            b.t.localPosition = b.lp;
            b.t.localRotation = b.lr;
            b.t.localScale = b.ls;
        }
    }

    /// <summary>�״� ���� ��ġ �״�� �ǻ츮�� ���� �� ���.</summary>
    public void ApplyAtCapturedWorldPose(Transform poseRoot)
        => Apply(poseRoot, worldRootPos, worldRootRot);
}
