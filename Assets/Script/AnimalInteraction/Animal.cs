using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class MaterialSwap
{
    public Material from;
    public Material to;
}

public class Animal : MonoBehaviour
{
    public string animalID;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Materials")]
    [SerializeField] private Renderer[] renderersToSwap;
    [SerializeField] private MaterialSwap[] materialSwaps;

    [Header("Objects To Remove On Heal")]
    [SerializeField] private GameObject[] objectsToDestroyOnHeal;

    [Header("Heal VFX")]
    [SerializeField] private GameObject healVFXPrefab;
    [SerializeField] private Vector3 vfxOffset = Vector3.zero;
    [SerializeField] private float vfxScale = 1f;
    [Header("Snail")]
    [SerializeField] private SnailAnimLight snailToChange;
    [SerializeField] private int healedSnailID;
    private bool healed = false;

    public void Heal()
    {
        if (healed)
            return;

        healed = true;

        if (snailToChange != null)
        {
            snailToChange.ChangeSnailID(healedSnailID);
        }

        SpawnHealVFX();

        if (animator != null)
        {
            animator.SetTrigger("Healed");
        }

        ApplyMaterialSwaps();
        DestroyHealObjects();
    }
    private void ApplyMaterialSwaps()
    {
        foreach (Renderer rend in renderersToSwap)
        {
            Material[] mats = rend.sharedMaterials;

            for (int i = 0; i < mats.Length; i++)
            {
                foreach (MaterialSwap swap in materialSwaps)
                {
                    if (mats[i] == swap.from)
                    {
                        mats[i] = swap.to;
                        break;
                    }
                }
            }

            rend.sharedMaterials = mats;
        }
    }

    private void DestroyHealObjects()
    {
        foreach (GameObject obj in objectsToDestroyOnHeal)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }

    private void SpawnHealVFX()
    {
        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);

        if (healVFXPrefab == null)
            return;

        GameObject vfx = Instantiate(
            healVFXPrefab,
            transform.position + vfxOffset,
            rotation
        );

        vfx.transform.localScale *= vfxScale;

        Destroy(vfx, 5f); // adjust to effect duration
    }
}