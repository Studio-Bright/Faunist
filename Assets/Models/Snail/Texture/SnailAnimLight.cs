using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailAnimLight : MonoBehaviour
{
    [Header("Group Settings")]
    public int snailID = 1;

    [Header("References")]
    public SkinnedMeshRenderer snailRenderer;

    [Header("Emission")]
    public float startEmission = 1f;
    public float peakEmission = 5f;

    [Header("Timing")]
    public float increaseDuration = 0.5f;
    public float decreaseDuration = 1f;

    private Material mat;
    private Color baseEmissionColor;

    private Coroutine currentAnimation;

    // Multiple snails can belong to the same ID
    private static Dictionary<int, List<SnailAnimLight>> snails =
        new Dictionary<int, List<SnailAnimLight>>();

    private static bool isLoopRunning = false;

    private void Awake()
    {
        if (!snails.ContainsKey(snailID))
        {
            snails.Add(snailID, new List<SnailAnimLight>());
        }

        snails[snailID].Add(this);
    }

    private void OnDestroy()
    {
        if (!snails.ContainsKey(snailID))
            return;

        snails[snailID].Remove(this);

        if (snails[snailID].Count == 0)
        {
            snails.Remove(snailID);
        }
    }

    private void Start()
    {
        mat = snailRenderer.material;

        mat.EnableKeyword("_EMISSION");

        baseEmissionColor = mat.GetColor("_EmissionColor");

        SetEmission(startEmission);

        isLoopRunning = true;

        if (snails.ContainsKey(1))
        {
            foreach (var snail in snails[1])
            {
                snail.StartSnail();
            }
        }
    }

    /*private void Update()
    {
        // Start loop with keyboard 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!isLoopRunning)
            {
                isLoopRunning = true;

                if (snails.ContainsKey(1))
                {
                    foreach (var snail in snails[1])
                    {
                        snail.StartSnail();
                    }
                }
            }
        }

        // Stop loop with keyboard 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isLoopRunning = false;
        }
    }*/

    public void StartSnail()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        currentAnimation = StartCoroutine(AnimateEmission());
    }

    private IEnumerator AnimateEmission()
    {
        float timer = 0f;

        // Fade up
        while (timer < increaseDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.SmoothStep(
                0f,
                1f,
                Mathf.Clamp01(timer / increaseDuration));

            SetEmission(Mathf.Lerp(startEmission, peakEmission, t));

            yield return null;
        }

        SetEmission(peakEmission);

        // Only one snail from the group should trigger the next group
        if (isLoopRunning && IsFirstInGroup())
        {
            int nextID = GetNextID();

            if (snails.ContainsKey(nextID))
            {
                foreach (var snail in snails[nextID])
                {
                    snail.StartSnail();
                }
            }
        }

        // Fade down
        timer = 0f;

        while (timer < decreaseDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.SmoothStep(
                0f,
                1f,
                Mathf.Clamp01(timer / decreaseDuration));

            SetEmission(Mathf.Lerp(peakEmission, startEmission, t));

            yield return null;
        }

        SetEmission(startEmission);

        currentAnimation = null;
    }

    private bool IsFirstInGroup()
    {
        return snails[snailID][0] == this;
    }

    private int GetNextID()
    {
        List<int> ids = new List<int>(snails.Keys);
        ids.Sort();

        int currentIndex = ids.IndexOf(snailID);

        if (currentIndex == ids.Count - 1)
        {
            return ids[0]; // loop back to first group
        }

        return ids[currentIndex + 1];
    }

    private void SetEmission(float strength)
    {
        mat.SetColor("_EmissionColor", baseEmissionColor * strength);
    }
    public void ChangeSnailID(int newID)
    {
        if (snailID == newID)
            return;

        // Remove from old group
        if (snails.ContainsKey(snailID))
        {
            snails[snailID].Remove(this);

            if (snails[snailID].Count == 0)
            {
                snails.Remove(snailID);
            }
        }

        // Assign new ID
        snailID = newID;

        // Add to new group
        if (!snails.ContainsKey(snailID))
        {
            snails.Add(snailID, new List<SnailAnimLight>());
        }

        snails[snailID].Add(this);
    }

}