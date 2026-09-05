using UnityEngine;

public class BellCall : MonoBehaviour
{
    public GameObject bellCanvas;
   

    public void TurnOffBellCanvas()
    {
        bellCanvas.SetActive(false);
    }

    public void TurnOnBellCanvas()
    {
        bellCanvas.SetActive(true);
    }
}
