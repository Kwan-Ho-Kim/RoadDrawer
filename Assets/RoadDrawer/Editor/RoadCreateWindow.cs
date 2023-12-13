using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// using Unity.VisualScripting;
using UnityEngine.UIElements;
using System;

public class RoadCreateWindow : EditorWindow
{

    public static RoadCreateWindow Instance {get; private set;}

    string[] toolbars = {"Disable", "Crosswalk notice", "Yield", "Pause"};
    string[] lane_toolbars = {"Disable", "White", "Yellow", "modify", "Others", "Segments"};
    string[] lane_tools = {"Disable", "White", "Yellow", "Others", "Segments","modify"};
    public int toolbar_index = 0;
    public int lane_toolbar_index = 0;
    public int lane_tool_index = 0;
    // public float segment_scale = 0;

    [MenuItem("Tools/RoadCreate")]
    static void Init()
    {
        RoadCreateWindow window =
            (RoadCreateWindow)EditorWindow.GetWindow(typeof(RoadCreateWindow));
        window.Show();
        
    }
    
    void OnGUI()
    {
        // GUILayout.Label("Marks", EditorStyles.boldLabel);
        // toolbar_index = GUILayout.Toolbar(toolbar_index, toolbars);

        // GUILayout.Space(15);

        GUILayout.Label("Lanes", EditorStyles.boldLabel);
        // lane_toolbar_index = GUILayout.Toolbar(lane_toolbar_index, lane_toolbars);

        lane_tool_index = GUILayout.SelectionGrid(lane_tool_index, lane_tools,3);
        lane_toolbar_index = Array.IndexOf(lane_toolbars, lane_tools[lane_tool_index]);
        


        Instance = this;
    }

    private void OnDisable()
    {
        toolbar_index = 0;
        lane_toolbar_index = 0;
    }
}
