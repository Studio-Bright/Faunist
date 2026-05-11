using UnityEngine;
using UnityEngine.UI;

public class ReputationUI : MonoBehaviour
{
    public Image permanentFill; // Image A
    public Image bufferFill;    // Image B (on top)

    private float baseValue;
    private float maxValue;
    private float duration;
    private float elapsed;
    private bool isRunning;

    public void SetBaseIfLower(float minValue)
    {
        if (baseValue < minValue)
        {
            baseValue = minValue;

            permanentFill.fillAmount = baseValue;
            bufferFill.fillAmount = baseValue;
        }
    }

    public void Initialize(float startValue)
    {
        baseValue = startValue;
        permanentFill.fillAmount = baseValue;
        bufferFill.fillAmount = baseValue;
    }

    public void StartDecay(float bonus, float time)
    {
        float effectiveBase = baseValue;

        // 🔥 If below 0.21 → use 0.21 ONLY for this encounter (temporary)
        if (baseValue < 0.21f)
        {
            effectiveBase = 0.21f;
        }

        maxValue = Mathf.Clamp01(effectiveBase + bonus);
        duration = time;

        elapsed = 0f;
        isRunning = true;

        bufferFill.fillAmount = maxValue;
    }

    void Update()
    {
        if (!isRunning) return;

        elapsed += Time.deltaTime;

        float t = Mathf.Clamp01(elapsed / duration);

        float targetBase = baseValue;

        if (baseValue < 0.21f)
            targetBase = 0.21f;

        float current = Mathf.Lerp(maxValue, targetBase, t);

        bufferFill.fillAmount = current;

        if (t >= 1f)
            isRunning = false;
    }

    // 🔥 THIS IS THE IMPORTANT PART
    public void CommitCurrentToPermanent()
    {
        float current = bufferFill.fillAmount;

        baseValue = current;

        permanentFill.fillAmount = baseValue;
        bufferFill.fillAmount = baseValue;

        isRunning = false;
    }

    public void StopDecay()
    {
        isRunning = false;
    }

    public float GetBaseValue()
    {
        return baseValue;
    }
}