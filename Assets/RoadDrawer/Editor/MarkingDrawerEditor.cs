using System;
using System.Collections;
using System.Collections.Generic;
// using Barmetler.RoadSystem;
using NUnit.Framework;
using PlasticPipe.PlasticProtocol.Messages;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//MyItem 스크립트의 에디터를 커스터마이징한다.
[CustomEditor(typeof(MarkingDrawer))]
// [CustomEditor(typeof(Menu))]
//여러 오브젝트를 선택했을 때 수정가능
[CanEditMultipleObjects]
public class MyRoadEditor : Editor
{
    // public MyRoad road { get; private set; }

    SerializedProperty item1;
    SerializedProperty item2;
    SerializedProperty item3;

    public GameObject crosswalk_notice;
    public GameObject yield;
    public GameObject pause;
    public GameObject marking_drawer;

    private RaycastHit prev_hit;

    void OnEnable()
    {
        item1 = serializedObject.FindProperty("CrosswalkNotice");
        item2 = serializedObject.FindProperty("Yield");
        item3 = serializedObject.FindProperty("Pause");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(item1);
        EditorGUILayout.PropertyField(item2);
        EditorGUILayout.PropertyField(item3);
        serializedObject.ApplyModifiedProperties();
        // Debug.Log(item.objectReferenceValue.GameObject().transform.position);
    }

    // object(MyRoad)가 활성화 되어 있어야 함
    private void OnSceneGUI()
    {
        marking_drawer = (target as MarkingDrawer).gameObject;
        crosswalk_notice = (target as MarkingDrawer).CrosswalkNotice;
        yield = (target as MarkingDrawer).Yield;
        pause = (target as MarkingDrawer).Pause;

        Event currentEvent = Event.current;
        
        // 마우스 왼쪽 버튼을 클릭하고, 이벤트가 마우스 클릭인 경우:
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {       

            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            RaycastHit hit;

            // Raycast를 사용하여 마우스 클릭 지점에 충돌이 있는지 확인합니다.
            if (Physics.Raycast(ray, out hit))
            {

                if(Vector3.Distance(hit.point, prev_hit.point) > 0.3)
                {
                    try
                    {
                        int tool_idx = RoadCreateWindow.Instance.toolbar_index;
                        CreateMark(tool_idx, hit);
                        
                    }
                    catch (NullReferenceException)
                    {
                        Debug.Log("try after activating a mark selection bar");
                    }
                
                }
                

                // Scene 뷰를 다시 그리도록 요청합니다.
                SceneView.RepaintAll();
            }
            
            prev_hit = hit;
        }

    }
    
    private void CreateMark(int tool_idx, RaycastHit hit)
    {
        GameObject newObj;
        // bool disable = false;
        switch (tool_idx)
        {
            case 0:
                // disable = true;
                break;
            case 1:                
                newObj = Instantiate(crosswalk_notice);
                newObj.transform.parent = marking_drawer.transform;
                newObj.transform.position = hit.point;
                Undo.RegisterCreatedObjectUndo(newObj, "Create New Object");
                Selection.activeGameObject = newObj;
                break;
            case 2:
                newObj = Instantiate(yield);
                newObj.transform.parent = marking_drawer.transform;
                newObj.transform.position = hit.point;
                Undo.RegisterCreatedObjectUndo(newObj, "Create New Object");
                Selection.activeGameObject = newObj;
                break;
            case 3:
                newObj = Instantiate(pause);
                newObj.transform.parent = marking_drawer.transform;
                newObj.transform.position = hit.point;
                Undo.RegisterCreatedObjectUndo(newObj, "Create New Object");
                Selection.activeGameObject = newObj;
                break;
            default:
                newObj = new GameObject("default");
                newObj.transform.parent = marking_drawer.transform;
                newObj.transform.position = hit.point;
                Undo.RegisterCreatedObjectUndo(newObj, "Create New Object");
                Selection.activeGameObject = newObj;
                break;

        }

    }

}
