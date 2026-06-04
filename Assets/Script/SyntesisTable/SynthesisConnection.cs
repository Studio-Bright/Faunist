using UnityEngine;

[System.Serializable]
public class SynthesisConnection
{
    [Header("Items Required")]
    public string itemA;
    public string itemB;

    [Header("Segment To Glow")]
    public PathSegment segment;

    [Header("Use center segment instead")]
    public bool useCenterSegment;
}