using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

[CustomEditor (typeof (ChunkLoader))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ChunkLoader mapGen = (ChunkLoader) target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            mapGen.ZeroChunk();
        }

    }
}
