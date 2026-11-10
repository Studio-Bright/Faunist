using System;
using UnityEngine;

[Serializable]
public class MaterialSwap
{
    public Material from;
    public Material to;
}

public class Animal : MonoBehaviour, IUniversalInteractable
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
    [SerializeField] private Vector3 vfxOffset;
    [SerializeField] private float vfxScale = 1f;

    [Header("Snail")]
    [SerializeField] private SnailAnimLight snailToChange;
    [SerializeField] private int healedSnailID;

    private bool healed;

    public bool ShowInteractionPrompt => !healed;

    public void Interact(PlayerInteractionContext context)
    {
        if (healed) return;

        PickupItem item = context.SelectedItem;
        if (item == null) return;

        PotionItem potion = item.GetComponent<PotionItem>();
        if (potion == null)
        {
            Debug.Log("This animal requires a potion.");
            return;
        }

        if (potion.targetAnimalID != animalID)
        {
            Debug.Log("Wrong potion.");
            return;
        }

        Heal();
        context.Inventory.RemoveSelected();
        Destroy(item.gameObject);

        AnimalEncounterManager manager = FindFirstObjectByType<AnimalEncounterManager>();
        if (manager != null) manager.OnAnimalHealed();
    }

    public void AlternateInteract(PlayerInteractionContext context) { }

    public void Heal()
    {
        if (healed) return;

        healed = true;

        if (snailToChange != null)
            snailToChange.ChangeSnailID(healedSnailID);

        SpawnHealVFX();

        if (animator != null)
            animator.SetTrigger("Healed");

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
            if (obj != null) Destroy(obj);
        }
    }

    private void SpawnHealVFX()
    {
        if (healVFXPrefab == null) return;

        GameObject vfx = Instantiate(
            healVFXPrefab,
            transform.position + vfxOffset,
            Quaternion.Euler(-90f, 0f, 0f)
        );

        vfx.transform.localScale *= vfxScale;
        Destroy(vfx, 5f);
    }
}

