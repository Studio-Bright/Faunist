using UnityEngine;

public class WaterTap : MonoBehaviour
{
    [Header("References")]
    public GameObject vfxTap;
    public WaterSource sinkSource;

    [Header("State")]
    public bool isPouring = false;

    public void TurnOnSink()
    {
        vfxTap.SetActive(true);

        isPouring = true;

        sinkSource.enabled = true;
    }

    public void TurnOffSink()
    {
        vfxTap.SetActive(false);

        isPouring = false;

        sinkSource.enabled = false;
    }

    public void Toggle()
    {
        if (isPouring)
            TurnOffSink();
        else
            TurnOnSink();
    }
}