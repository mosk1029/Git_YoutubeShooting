using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]      // 이 에디터 스크립트가 어떤 스크립트를 다루는지 명시
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator map = target as MapGenerator;

        if(DrawDefaultInspector())     // bool 값을 리턴하는데 인스펙터에서 값이 갱신될때만 true를 반환
        {
            map.GenerateMap();
        }

        // 수동으로 버튼을 눌러서 맵 생성
        if(GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }
    }
}
