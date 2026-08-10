using UnityEngine;

[CreateAssetMenu(menuName = "Vet Sim/Symptom Compatibility")]
public class SymptomCompatibilityMatrix : ScriptableObject
{
    public SymptomDatabase database;

    [SerializeField]
    private bool[] compatibility;

    public bool IsCompatible(int a, int b)
    {
        int count = database.symptoms.Count;

        if (compatibility == null)
            return true;

        int index = a * count + b;

        if (index >= compatibility.Length)
            return true;

        return compatibility[index];
    }

    public void SetCompatibility(int a, int b, bool value)
    {
        int count = database.symptoms.Count;

        compatibility[a * count + b] = value;
        compatibility[b * count + a] = value;
    }

    public void Resize()
    {
        int count = database.symptoms.Count;

        if (compatibility == null ||
            compatibility.Length != count * count)
        {
            bool[] old = compatibility;

            compatibility = new bool[count * count];

            for (int i = 0; i < compatibility.Length; i++)
            {
                compatibility[i] = true;
            }

            if (old != null)
            {
                int oldSize = Mathf.RoundToInt(
                    Mathf.Sqrt(old.Length));

                for (int y = 0; y < Mathf.Min(oldSize, count); y++)
                {
                    for (int x = 0; x < Mathf.Min(oldSize, count); x++)
                    {
                        compatibility[y * count + x] =
                            old[y * oldSize + x];
                    }
                }
            }
        }
    }
}