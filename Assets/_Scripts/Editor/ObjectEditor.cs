using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Object))]
public class ObjectEditor : Editor
{
    private Object objectToEdit;

    public override void OnInspectorGUI()
    {
        objectToEdit = (Object)target;

        base.OnInspectorGUI();

        GUILayout.Space(20);

        if (GUILayout.Button("Create default category feature"))
        {
            objectToEdit.SetAutomaticCategoryFeatures();
            EditorUtility.SetDirty(objectToEdit);
        }
        if (GUILayout.Button("Create 1 random trait feature"))
        {
            objectToEdit.CreateOneRandomFeature();
            EditorUtility.SetDirty(objectToEdit);
        }
        if (GUILayout.Button("Randomize last feature values"))
        {
            objectToEdit.RandomizeLastFeatureValues();
            EditorUtility.SetDirty(objectToEdit);
        }
    }
}
