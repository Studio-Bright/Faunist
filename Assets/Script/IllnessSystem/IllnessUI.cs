using TMPro;
using UnityEngine;

public class IllnessUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void Show(IllnessDatabase database)
    {
        string result = "";

        foreach (var illness in database.illnesses)
        {
            result += illness.illnessName + "\n";

            foreach (var symptom in illness.symptoms)
            {
                result += "- " + symptom.symptomName + "\n";
            }

            result += "\n";
        }

        text.text = result;
    }
}