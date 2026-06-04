using UnityEngine;

public class BellAnimation : MonoBehaviour
{
    public Animator bellAnimator;

    public void PlayRing()
    {
        bellAnimator.enabled = true;

        bellAnimator.SetTrigger("Ring");
    }

    public void StopRing()
    {
        bellAnimator.ResetTrigger("Ring");

        bellAnimator.CrossFade("Empty", 0.1f);
    }

    public void EnableAnimator()
    {
        bellAnimator.enabled = true;
    }
}