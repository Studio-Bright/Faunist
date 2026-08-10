using UnityEngine;

public class AnimalAttachmentPoints : MonoBehaviour
{
    public Transform head;
    public Transform aboveHead;
    public Transform ears;
    public Transform back;
    public Transform butt;

    public Transform GetPoint(AttachmentPointType pointType)
    {
        switch (pointType)
        {
            case AttachmentPointType.Head:
                return head;

            case AttachmentPointType.AboveHead:
                return aboveHead;

            case AttachmentPointType.Ears:
                return ears;

            case AttachmentPointType.Back:
                return back;

            case AttachmentPointType.Butt:
                return butt;

            default:
                return null;
        }
    }
}