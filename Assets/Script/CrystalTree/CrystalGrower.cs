using System.Collections.Generic;
using UnityEngine;

public class CrystalGrower : MonoBehaviour
{
    [Header("Setup")]
    public GameObject segmentPrefab;
    public Transform spawnRoot;

    [Header("Growth Settings")]
    public float segmentLength = 0.5f;
    public int maxSegments = 30;
    public int branchesPerStep = 2; // сколько веток максимум

    private List<Transform> tips = new List<Transform>();
    private int totalSegments = 0;

    void Start()
    {
        // стартовый сегмент
        Transform start = Instantiate(
            segmentPrefab,
            spawnRoot.position,
            Quaternion.identity,
            spawnRoot
        ).transform;

        tips.Add(start);
        totalSegments = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Grow();
        }
    }

    void Grow()
    {
        if (totalSegments >= maxSegments) return;

        List<Transform> newTips = new List<Transform>();

        foreach (Transform tip in tips)
        {
            int branchCount = Random.Range(1, branchesPerStep + 1);

            for (int i = 0; i < branchCount; i++)
            {
                if (totalSegments >= maxSegments) break;

                float angle = Random.Range(-40f, 40f);

                Quaternion rotation = tip.rotation * Quaternion.Euler(angle, 0f, 0f);

                Vector3 position = tip.position + (rotation * Vector3.up * segmentLength);

                Transform segment = Instantiate(
                    segmentPrefab,
                    position,
                    rotation,
                    spawnRoot
                ).transform;

                newTips.Add(segment);
                totalSegments++;
            }
        }

        // теперь растём только от новых концов
        tips = newTips;
    }
}