using UnityEngine;
using UnityEditor;

namespace ML.MazeGame{
    [CustomEditor(typeof(LevelButtonGenerate))]
    public class LevelButtonGenerate_Editor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            LevelButtonGenerate m_script = (LevelButtonGenerate)target;

            GUILayout.Space(10);
            GUILayout.Label ("Use functions below for generating level buttons");
            GUILayout.Space(10);
            if(GUILayout.Button("Clear All Levels"))      m_script.ClearAllLevel();
            if(GUILayout.Button("Generate Levels"))       m_script.GenerateLevel();
            if(GUILayout.Button("Random Stage Unlock"))   m_script.RandomStageUnlock();
            if(GUILayout.Button("Reset Stages"))          m_script.ResetStages();
            if(GUILayout.Button("Save Stages as Prefab")) {
                string localPath = m_script.prefabPath + m_script.prefabRoot.name + ".prefab";
                PrefabUtility.SaveAsPrefabAssetAndConnect(m_script.prefabRoot, localPath, InteractionMode.AutomatedAction);
                PrefabUtility.UnpackPrefabInstance(m_script.prefabRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
        }
    }
}
