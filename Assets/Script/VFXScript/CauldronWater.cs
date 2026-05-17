using UnityEngine;
using System.Collections;

public class CauldronWater : MonoBehaviour
{
    [Header("Water Height")]
    public float lowestY;
    public float highestY;

    [Header("Animation")]
    public float riseDuration = 1f;

    private Coroutine currentRoutine;

    private void Start()
    {
        SetWaterHeight(lowestY);
        gameObject.SetActive(false);
    }

    public void FillWater()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        gameObject.SetActive(true);

        currentRoutine = StartCoroutine(
            MoveWater(highestY)
        );
    }

    public void EmptyWater()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        SetWaterHeight(lowestY);

        gameObject.SetActive(false);
    }

    private IEnumerator MoveWater(float targetY)
    {
        Vector3 startPos = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / riseDuration;

            Vector3 pos = startPos;
            pos.y = Mathf.Lerp(startPos.y, targetY, t);

            transform.localPosition = pos;

            yield return null;
        }

        SetWaterHeight(targetY);
    }

    private void SetWaterHeight(float y)
    {
        Vector3 pos = transform.localPosition;
        pos.y = y;
        transform.localPosition = pos;
    }
}