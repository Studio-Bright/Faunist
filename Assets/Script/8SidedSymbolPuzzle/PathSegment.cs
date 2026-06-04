using UnityEngine;

public class PathSegment : MonoBehaviour
{
    [Header("Connected Nodes")]
    public Node nodeA;
    public Node nodeB;

    [Header("Visuals")]
    [SerializeField] private Renderer rend;

    [Header("Emission")]
    [SerializeField] private Color inactiveEmission = Color.black;
    [SerializeField] private Color activeEmission = new Color(5f, 4f, 0f);

    private Material mat;

    void Awake()
    {
        mat = rend.material;
        mat.EnableKeyword("_EMISSION");
        SetActive(false);
    }

    public void SetActive(bool state)
    {
        Color emission = state ? activeEmission : inactiveEmission;

        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", emission * 5f);
    }


    public void ResetSegment()
    {
        SetActive(false);
    }
}