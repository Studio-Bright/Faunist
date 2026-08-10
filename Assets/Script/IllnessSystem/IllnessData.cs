using System.Collections.Generic;

[System.Serializable]
public class IllnessData
{
    public string illnessName;

    public List<SymptomData> symptoms = new();
}