using UnityEngine;
using UnityEngine.UI;

public class ReputationBar : MonoBehaviour
{
    public Image fillImage;

    public float totalTime = 60f; 

    private float currentTime;

    

    void Start()
    {
        currentTime = totalTime;
        fillImage.fillAmount = 1f;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            fillImage.fillAmount = currentTime / totalTime;
        }
        else
        {
            fillImage.fillAmount = 0f;
        }
    }
}
