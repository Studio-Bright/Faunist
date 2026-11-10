using UnityEngine;

public class PlantCreation : MonoBehaviour
{
    [SerializeField] private Transform plantTransform;

    [SerializeField] private float minScale = 0f;
    [SerializeField] private float targetScale = 0f;
    [SerializeField] public float growSpeed = 0.2f;

    private void Start()
    {
        plantTransform.localScale = Vector3.one * minScale;
       
    }

    private void Update()
    {
        PlantGrow();
    }

    private void PlantGrow()
    {
        float currentScale = plantTransform.localScale.x;

        float newScale = Mathf.MoveTowards(currentScale, targetScale, growSpeed * Time.deltaTime);

        plantTransform.localScale = Vector3.one * newScale;
    }
}

