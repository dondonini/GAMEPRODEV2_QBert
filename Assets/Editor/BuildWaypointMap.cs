using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(PathManager))]
public class BuildWaypointMap : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Waypoint Map Builder", GUILayout.Width(255)))
        {
            BuildWaypointWindow window = (BuildWaypointWindow)EditorWindow.GetWindow(typeof(BuildWaypointWindow));
            window.Init();
        }

        SceneView.RepaintAll();
        DrawDefaultInspector();
    }

}
