using UnityEngine;
using UnityEditor;

namespace ML.MazeGame{
    [CustomEditor(typeof(PathFinding))]
    public class PathFinding_Editor : Editor
    {   
      public override void OnInspectorGUI()
      {
        base.OnInspectorGUI();

        PathFinding m_script = (PathFinding)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Find Path")) m_script.FindPath();
      }
    }
}
