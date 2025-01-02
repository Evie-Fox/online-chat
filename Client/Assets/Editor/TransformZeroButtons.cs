using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class TransformZeroButtons : Editor
{
    private Editor defaultEditor;

    private void OnEnable()
    {
        defaultEditor = Editor.CreateEditor(target, typeof(Editor).Assembly.GetType("UnityEditor.TransformInspector"));
    }

    private void OnDisable()
    {
        DestroyImmediate(defaultEditor);
    }

    public override void OnInspectorGUI()
    {
        if (defaultEditor != null)
        {
            defaultEditor.OnInspectorGUI();
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Zero position"))
        {
            Transform transform = (Transform)target;
            Undo.RecordObject(transform, "Zero Position");
            transform.localPosition = Vector3.zero;
        }

        if (GUILayout.Button("Zero rotation"))
        {
            Transform transform = (Transform)target;
            Undo.RecordObject(transform, "Zero Rotation");
            transform.localRotation = Quaternion.identity;
        }

        if (GUILayout.Button("Move to view"))
        {
            Transform transform = (Transform)target;
            Transform camTransform = SceneView.lastActiveSceneView.camera.transform;
            Undo.RecordObject(transform, "Move to View");
            transform.position = camTransform.position + camTransform.forward * 3f;
        }

        GUILayout.EndHorizontal();
    }
}
