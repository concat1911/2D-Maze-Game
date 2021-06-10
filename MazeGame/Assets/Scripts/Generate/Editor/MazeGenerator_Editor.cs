using UnityEngine;
using UnityEditor;

namespace ML.MazeGame
{
  [CustomEditor(typeof(MazeGenerator))]
  public class MazeGenerator_Editor : Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();

      MazeGenerator m_script = (MazeGenerator)target;

      GUILayout.Space(10);

      if (GUILayout.Button("Generate Maze")) m_script.Generate();
    }
  }
}
