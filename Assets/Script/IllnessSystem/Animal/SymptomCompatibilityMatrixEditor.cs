using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SymptomCompatibilityMatrix))]
public class SymptomCompatibilityMatrixEditor : Editor
{
    private SymptomCompatibilityMatrix matrix;

    private void OnEnable()
    {
        matrix = (SymptomCompatibilityMatrix)target;

        if (matrix.database != null)
            matrix.Resize();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        if (matrix.database == null)
            return;

        matrix.Resize();

        GUILayout.Space(15);

        EditorGUILayout.LabelField(
            "Compatibility Matrix",
            EditorStyles.boldLabel);

        var symptoms = matrix.database.symptoms;

        int count = symptoms.Count;

        GUILayout.BeginHorizontal();

        GUILayout.Space(150);

        for (int x = 0; x < count; x++)
        {
            DrawVerticalLabel(
                symptoms[x].symptomName,
                25,
                120);
        }

        GUILayout.EndHorizontal();


        for (int y = 0; y < count; y++)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(
                symptoms[y].symptomName,
                GUILayout.Width(150));

            for (int x = 0; x < count; x++)
            {
                bool value =
                    matrix.IsCompatible(y, x);

                bool newValue =
                    GUILayout.Toggle(
                        value,
                        "",
                        GUILayout.Width(25));

                if (newValue != value)
                {
                    Undo.RecordObject(matrix,
                        "Modify Compatibility");

                    matrix.SetCompatibility(
                        y,
                        x,
                        newValue);

                    EditorUtility.SetDirty(matrix);
                }
            }

            GUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawVerticalLabel(string text, float width, float height)
    {
        Rect rect = GUILayoutUtility.GetRect(width, height);

        Matrix4x4 oldMatrix = GUI.matrix;

        GUIUtility.RotateAroundPivot(
            -90,
            new Vector2(rect.x, rect.y));

        GUI.Label(
            new Rect(
                rect.x - height,
                rect.y,
                height,
                width),
            text);

        GUI.matrix = oldMatrix;
    }
}