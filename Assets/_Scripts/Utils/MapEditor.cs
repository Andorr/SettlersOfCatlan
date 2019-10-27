using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;

[CustomEditor(typeof(MapController))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI ()
	{
		MapController map = target as MapController;
		if(DrawDefaultInspector())
		{
			map.GenerateMap();
		}

		if(GUILayout.Button("Generate Map"))
		{
			map.GenerateMap();
		}
	}
}
