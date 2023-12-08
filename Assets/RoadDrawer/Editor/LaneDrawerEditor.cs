using System;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//MyItem 스크립트의 에디터를 커스터마이징한다.
[CustomEditor(typeof(LaneDrawer))]
// [CustomEditor(typeof(Menu))]
//여러 오브젝트를 선택했을 때 수정가능
[CanEditMultipleObjects]
public class LaneDrawerEditor : Editor
{

    SerializedProperty item1;
    SerializedProperty item2;
    SerializedProperty item3;
    SerializedProperty item4;
    SerializedProperty item5;

    public GameObject lane_drawer;

    private RaycastHit prev_hit;

    private List<GameObject> nodes = new List<GameObject>();

    private bool enterKeyPressed = false;
    private bool node_active_check = false;

    public float segment_scale = 4.5f;

    
    void OnEnable()
    {
        item1 = serializedObject.FindProperty("WhiteLane");
        item2 = serializedObject.FindProperty("YellowLane");
        item3 = serializedObject.FindProperty("Others");
        item4 = serializedObject.FindProperty("Segments");
        item5 = serializedObject.FindProperty("segment_scale");

        EditorTagLayerHelper.AddNewTag("node");
        EditorTagLayerHelper.AddNewTag("lane");
        EditorTagLayerHelper.AddNewTag("Segments");
        EditorTagLayerHelper.AddNewTag("Others");
        EditorTagLayerHelper.AddNewTag("Yellow lane");
        EditorTagLayerHelper.AddNewTag("White lane");
        

    }
    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(item1);
        EditorGUILayout.PropertyField(item2);
        EditorGUILayout.PropertyField(item3);
        EditorGUILayout.PropertyField(item4);
        EditorGUILayout.PropertyField(item5);
        
        serializedObject.ApplyModifiedProperties();
        // Debug.Log(item1.objectReferenceValue.GameObject().transform.position);
    }

    // object(MyRoad)가 활성화 되어 있어야 함
    private void OnSceneGUI()
    {
        segment_scale = item5.floatValue;
        lane_drawer = (target as LaneDrawer).gameObject;        // lane creator

        Event currentEvent = Event.current;

        int lane_toolbar_index = RoadCreateWindow.Instance.lane_toolbar_index;
        

        // 마우스 왼쪽 버튼을 클릭하고, 이벤트가 마우스 클릭인 경우:
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {       

            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            RaycastHit hit;

            // Raycast를 사용하여 마우스 클릭 지점에 충돌이 있는지 확인합니다.
            if (Physics.Raycast(ray, out hit))
            {

                // if(Vector3.Distance(hit.point, prev_hit.point) > 0.3)
                // {
                //     if (RoadCreateWindow.Instance.lane_toolbar_index != 0)    // disable이 아니면
                //     {
                //         CreateNodes(hit);
                //     }
                // }

                if (RoadCreateWindow.Instance.lane_toolbar_index == 1 || RoadCreateWindow.Instance.lane_toolbar_index == 2 || RoadCreateWindow.Instance.lane_toolbar_index == 4 ||RoadCreateWindow.Instance.lane_toolbar_index == 5)    // white lane or yellow lane or others
                {
                    CreateNodes(hit);
                }
            
                
                
                // Scene 뷰를 다시 그리도록 요청합니다.
                SceneView.RepaintAll();
            }

            prev_hit = hit;
            
        }


        
        // 엔터키 눌렸을 때 통합
        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Return && !enterKeyPressed)
        {
            enterKeyPressed = true;

            if (lane_toolbar_index == 1)        // White
            {
                GameObject white_lane = item1.objectReferenceValue.GameObject();
                GameObject parent_lane = new GameObject("White lane");
                parent_lane.transform.tag = "White lane";

                Undo.RegisterCreatedObjectUndo(white_lane, "Create New Object");
                Undo.RegisterCreatedObjectUndo(parent_lane, "Create New Object");
                CreateLane(parent_lane, white_lane);
            }
            if (lane_toolbar_index == 2)        // Yellow
            {
                GameObject yellow_lane = item2.objectReferenceValue.GameObject();
                GameObject parent_lane = new GameObject("Yellow lane");
                parent_lane.transform.tag = "Yellow lane";

                Undo.RegisterCreatedObjectUndo(yellow_lane, "Create New Object");
                Undo.RegisterCreatedObjectUndo(parent_lane, "Create New Object");

                CreateLane(parent_lane, yellow_lane);
            }
            if (lane_toolbar_index == 4)        // Others
            {
                GameObject other_lane = item3.objectReferenceValue.GameObject();
                GameObject parent_lane = new GameObject("others");
                parent_lane.transform.tag = "Others";

                Undo.RegisterCreatedObjectUndo(other_lane, "Create New Object");
                Undo.RegisterCreatedObjectUndo(parent_lane, "Create New Object");

                CreateLane(parent_lane, other_lane);
            }
            if (lane_toolbar_index == 5)
            {
                GameObject segment_lane = item4.objectReferenceValue.GameObject();
                GameObject parent_lane = new GameObject("Segments");
                parent_lane.transform.tag = "Segments";

                Undo.RegisterCreatedObjectUndo(segment_lane, "Create New Object");
                Undo.RegisterCreatedObjectUndo(parent_lane, "Create New Object");
                
                CreateSegmentLane(parent_lane, segment_lane);
            }


            nodes.Clear();

            
        }
        if (currentEvent.type == EventType.KeyUp && currentEvent.keyCode == KeyCode.Return)
        {
            enterKeyPressed = false;
        }

        
        // Modify
        GameObject[] all_nodes = GameObject.FindGameObjectsWithTag("node"); // 같은 태그를 가진 Object들을 GameObject[] 형태로 반환
        if (lane_toolbar_index == 3)
        {
            if (node_active_check == false)
            {
                // 노드 활성화
                foreach (GameObject each_node in all_nodes)
                {
                    MeshRenderer tmp_mesh = each_node.GetComponent<MeshRenderer>();
                    tmp_mesh.enabled = true;
                }
                node_active_check = true;
            }
            

        }
        else
        {
            if (node_active_check)
            {   
                // 노드 비활성화
                foreach (GameObject each_node in all_nodes)
                {
                    MeshRenderer tmp_mesh = each_node.GetComponent<MeshRenderer>();
                    tmp_mesh.enabled = false;
                }
                node_active_check = false;
            }            
            
        }

        // node 변화에 대한 수정
        if(lane_toolbar_index == 3 && Selection.activeGameObject.tag == "node")
        {
            ModifyLane(Selection.activeGameObject.transform.parent.gameObject, Selection.activeGameObject);
            
            
        }
        

    }
    

    private void CreateNodes(RaycastHit hit)
    {
        // GameObject node = new GameObject("node");
        GameObject node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        node.name = "node";
        node.tag = "node";
        node.transform.localScale = new Vector3(1f, 1f, 1f);
        SphereCollider sphereCollider = node.GetComponent<SphereCollider>();
        DestroyImmediate(sphereCollider);
        nodes.Add(node);

        node.transform.parent = lane_drawer.transform;
        node.transform.position = hit.point;
        Undo.RegisterCreatedObjectUndo(node, "Create New Object");
    }

    private void CreateSegmentLane(GameObject parent_lane, GameObject lane)        // lane : lane module, parent_lane : White lane or Yellow lane
    {
        parent_lane.transform.parent = lane_drawer.transform;

        int i = 0;
        foreach (GameObject node in nodes)
        {
            node.transform.parent = parent_lane.transform;
            if (i != nodes.Count - 1)
            {
                Vector3 lane_direction =  nodes[i+1].transform.position - node.transform.position;

                float num_segment = (lane_direction.magnitude / segment_scale - 1) / 2;


                for (int j = 0; j < num_segment; j++)
                {
                    GameObject newLaneModule = Instantiate(lane);
                    newLaneModule.tag = "lane";
                    newLaneModule.transform.parent = parent_lane.transform;

                    newLaneModule.transform.position = node.transform.position + lane_direction/lane_direction.magnitude * segment_scale * (1 + 4 * j) / 2;
                    newLaneModule.transform.rotation = Quaternion.LookRotation(lane_direction);
                    newLaneModule.transform.localScale = new Vector3(newLaneModule.transform.localScale.x, newLaneModule.transform.localScale.y, -segment_scale);

                    Undo.RegisterCreatedObjectUndo(newLaneModule, "Create New Object");
                }
                
            }

            i++;
        }
        if(i==0 || i==1)    // 예외처리
        {
            throw new Exception("두 개 이상의 노드를 선택하세요");
        }

    }

    private void CreateLane(GameObject parent_lane, GameObject lane)        // lane : lane module, parent_lane : White lane or Yellow lane
    {
        parent_lane.transform.parent = lane_drawer.transform;

        int i = 0;
        foreach (GameObject node in nodes)
        {
            node.transform.parent = parent_lane.transform;
            if (i != nodes.Count - 1)
            {
                GameObject newLaneModule = Instantiate(lane);
                newLaneModule.tag = "lane";
                newLaneModule.transform.parent = parent_lane.transform;
                
                CalLaneFromTwoNode(node, newLaneModule, nodes[i+1]);

                Undo.RegisterCreatedObjectUndo(newLaneModule, "Create New Object");
            }

            i++;
        }
        if(i==0 || i==1)    // 예외처리
        {
            throw new Exception("두 개 이상의 노드를 선택하세요");
        }

    }

    private void ModifyLane(GameObject parent_lane, GameObject node)
    {
        // List<GameObject> objects = new List<GameObject>();
        int childCount = parent_lane.transform.childCount;
        
        for(int i = 0; i < parent_lane.transform.childCount; i++)
        {
            // objects.Add(parent_lane.transform.GetChild(i).gameObject);
            if(node == parent_lane.transform.GetChild(i).gameObject)
            {
                if (parent_lane.transform.tag == "Segments")
                {
                    if(i==0)        // 첫 번째 노드
                    {
                        List<GameObject> segments = new List<GameObject>();
                        int next_node_idx = 0;

                        for (int j = i+1; parent_lane.transform.GetChild(j).gameObject.tag != "node"; j++)
                        {
                            segments.Add(parent_lane.transform.GetChild(j).gameObject);
                            next_node_idx = j + 1;
                        }

                        if (parent_lane.transform.GetChild(next_node_idx).gameObject.tag == "node" && next_node_idx != 0)
                        {
                            Vector3 lane_direction =  parent_lane.transform.GetChild(next_node_idx).gameObject.transform.position - node.transform.position;

                            float num_segment = (lane_direction.magnitude / Mathf.Abs(segments[0].transform.localScale.z) - 1) / 2;

                            if (segments.Count < (int)num_segment + 1)
                            {
                                GameObject extra_segment = Instantiate(segments[0]);
                                extra_segment.transform.SetParent(parent_lane.transform, true);
                                extra_segment.transform.SetSiblingIndex(next_node_idx);
                                segments.Add(extra_segment);
                            }
                            else if (segments.Count > (int)num_segment + 1)
                            {
                                DestroyImmediate(segments[segments.Count - 1]);
                                segments.Remove(segments[segments.Count - 1]);

                            }

                            int k = 0;
                            foreach(GameObject lane in segments)
                            {
                                lane.transform.position = node.transform.position + lane_direction/lane_direction.magnitude * Mathf.Abs(lane.transform.localScale.z) * (1 + 4 * k) / 2;
                                lane.transform.rotation = Quaternion.LookRotation(lane_direction);
                                lane.transform.localScale = new Vector3(lane.transform.localScale.x, lane.transform.localScale.y, lane.transform.localScale.z);
                                k++;
                            }
                            
                            
                        }
                        else
                        {
                            throw new Exception("Check Segment object, it can be no segment!");
                        }
                    }
                    else if(i == parent_lane.transform.childCount - 1)      // 마지막 노드
                    {
                        List<GameObject> segments = new List<GameObject>();
                        int prev_node_idx = parent_lane.transform.childCount - 1;

                        for (int j = i-1; parent_lane.transform.GetChild(j).gameObject.tag != "node"; j--)
                        {
                            segments.Add(parent_lane.transform.GetChild(j).gameObject);
                            prev_node_idx = j - 1;
                        }

                        if (parent_lane.transform.GetChild(prev_node_idx).gameObject.tag == "node" && prev_node_idx != parent_lane.transform.childCount - 1)
                        {
                            Vector3 lane_direction = node.transform.position - parent_lane.transform.GetChild(prev_node_idx).gameObject.transform.position;

                            float num_segment = (lane_direction.magnitude / Mathf.Abs(segments[0].transform.localScale.z) - 1) / 2;

                            if (segments.Count < (int)num_segment + 1)
                            {
                                GameObject extra_segment = Instantiate(segments[0]);
                                extra_segment.transform.SetParent(parent_lane.transform, true);
                                extra_segment.transform.SetSiblingIndex(prev_node_idx+1);
                                segments.Add(extra_segment);
                            }
                            else if (segments.Count > (int)num_segment + 1)
                            {
                                DestroyImmediate(segments[segments.Count - 1]);
                                segments.Remove(segments[segments.Count - 1]);

                            }

                            int k = 0;
                            foreach(GameObject lane in segments)
                            {
                                lane.transform.position = parent_lane.transform.GetChild(prev_node_idx).gameObject.transform.position + lane_direction/lane_direction.magnitude * Mathf.Abs(lane.transform.localScale.z) * (1 + 4 * k) / 2;
                                lane.transform.rotation = Quaternion.LookRotation(lane_direction);
                                lane.transform.localScale = new Vector3(lane.transform.localScale.x, lane.transform.localScale.y, lane.transform.localScale.z);
                                k++;
                            }
                            
                            
                        }
                        else
                        {
                            throw new Exception("Check Segment object");
                        }
                    }
                    else
                    {
                        // 다음 노드까지 segments
                        List<GameObject> segments = new List<GameObject>();
                        int next_node_idx = 0;

                        for (int j = i+1; parent_lane.transform.GetChild(j).gameObject.tag != "node"; j++)
                        {
                            segments.Add(parent_lane.transform.GetChild(j).gameObject);
                            next_node_idx = j + 1;
                        }

                        if (parent_lane.transform.GetChild(next_node_idx).gameObject.tag == "node" && next_node_idx != 0)
                        {
                            Vector3 lane_direction =  parent_lane.transform.GetChild(next_node_idx).gameObject.transform.position - node.transform.position;

                            float num_segment = (lane_direction.magnitude / Mathf.Abs(segments[0].transform.localScale.z) - 1) / 2;

                            if (segments.Count < (int)num_segment + 1)
                            {
                                GameObject extra_segment = Instantiate(segments[0]);
                                extra_segment.transform.SetParent(parent_lane.transform, true);
                                extra_segment.transform.SetSiblingIndex(next_node_idx);
                                segments.Add(extra_segment);
                            }
                            else if (segments.Count > (int)num_segment + 1)
                            {
                                DestroyImmediate(segments[segments.Count - 1]);
                                segments.Remove(segments[segments.Count - 1]);

                            }

                            int k = 0;
                            foreach(GameObject lane in segments)
                            {
                                lane.transform.position = node.transform.position + lane_direction/lane_direction.magnitude * Mathf.Abs(lane.transform.localScale.z) * (1 + 4 * k) / 2;
                                lane.transform.rotation = Quaternion.LookRotation(lane_direction);
                                lane.transform.localScale = new Vector3(lane.transform.localScale.x, lane.transform.localScale.y, lane.transform.localScale.z);
                                k++;
                            }
                        }
                        else
                        {
                            throw new Exception("Check Segment object");
                        }
                        
                        // 이전 노드부터 segments
                        segments.Clear();
                        int prev_node_idx = parent_lane.transform.childCount - 1;

                        for (int j = i-1; parent_lane.transform.GetChild(j).gameObject.tag != "node"; j--)
                        {
                            segments.Add(parent_lane.transform.GetChild(j).gameObject);
                            prev_node_idx = j - 1;
                        }

                        if (parent_lane.transform.GetChild(prev_node_idx).gameObject.tag == "node" && prev_node_idx != parent_lane.transform.childCount - 1)
                        {
                            Vector3 lane_direction = node.transform.position - parent_lane.transform.GetChild(prev_node_idx).gameObject.transform.position;

                            float num_segment = (lane_direction.magnitude / Mathf.Abs(segments[0].transform.localScale.z) - 1) / 2;

                            if (segments.Count < (int)num_segment + 1)
                            {
                                GameObject extra_segment = Instantiate(segments[0]);
                                extra_segment.transform.SetParent(parent_lane.transform, true);
                                extra_segment.transform.SetSiblingIndex(prev_node_idx+1);
                                segments.Add(extra_segment);
                            }
                            else if (segments.Count > (int)num_segment + 1)
                            {
                                DestroyImmediate(segments[segments.Count - 1]);
                                segments.Remove(segments[segments.Count - 1]);

                            }

                            int k = 0;
                            foreach(GameObject lane in segments)
                            {
                                lane.transform.position = parent_lane.transform.GetChild(prev_node_idx).gameObject.transform.position + lane_direction/lane_direction.magnitude * Mathf.Abs(lane.transform.localScale.z) * (1 + 4 * k) / 2;
                                lane.transform.rotation = Quaternion.LookRotation(lane_direction);
                                lane.transform.localScale = new Vector3(lane.transform.localScale.x, lane.transform.localScale.y, lane.transform.localScale.z);
                                k++;
                            }
                        }
                        else
                        {
                            throw new Exception("Check Segment object");
                        }



                    }
                }
                else
                {
                    if(i==0)        // 첫 번째 노드
                    {
                        GameObject next_lane = parent_lane.transform.GetChild(i+1).gameObject;
                        GameObject next_node = parent_lane.transform.GetChild(i+2).gameObject;
                        CalLaneFromTwoNode(node, next_lane, next_node);
                    }
                    else if(i == childCount - 1)      // 마지막 노드
                    {
                        GameObject prev_node = parent_lane.transform.GetChild(i-2).gameObject;
                        GameObject prev_lane = parent_lane.transform.GetChild(i-1).gameObject;
                        CalLaneFromTwoNode(prev_node, prev_lane, node);
                    }
                    else
                    {
                        GameObject prev_node = parent_lane.transform.GetChild(i-2).gameObject;
                        GameObject prev_lane = parent_lane.transform.GetChild(i-1).gameObject;
                        CalLaneFromTwoNode(prev_node, prev_lane, node);

                        GameObject next_lane = parent_lane.transform.GetChild(i+1).gameObject;
                        GameObject next_node = parent_lane.transform.GetChild(i+2).gameObject;
                        CalLaneFromTwoNode(node, next_lane, next_node);

                    }
                }
                
            }
        }
    }

    private void CalLaneFromTwoNode(GameObject node1, GameObject lane, GameObject node2)
    {
        lane.transform.position = (node1.transform.position + node2.transform.position) / 2;
        Vector3 lane_direction = node1.transform.position - node2.transform.position;                
        lane.transform.rotation = Quaternion.LookRotation(lane_direction);
        lane.transform.localScale = new Vector3(lane.transform.localScale.x, lane.transform.localScale.y, lane_direction.magnitude);
    }

}
