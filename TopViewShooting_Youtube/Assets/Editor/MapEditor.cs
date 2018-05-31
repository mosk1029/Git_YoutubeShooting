using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]      // 이 에디터 스크립트가 어떤 스크립트를 다루는지 명시
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator map = target as MapGenerator;

        map.GenerateMap();
    }

}
