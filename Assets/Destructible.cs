using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject DestructVersion;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Instantiate(DestructVersion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
