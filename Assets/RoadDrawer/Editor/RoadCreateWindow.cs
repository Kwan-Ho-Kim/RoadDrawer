using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// using Unity.VisualScripting;
using UnityEngine.UIElements;

public class RoadCreateWindow : EditorWindow
{

    public static RoadCreateWindow Instance {get; private set;}

    string[] toolbars = {"Disable", "Crosswalk notice", "Yield", "Pause"};
    string[] lane_toolbars = {"Disable", "White", "Yellow", "modify", "Others", "Segments"};
    public int toolbar_index = 0;
    public int lane_toolbar_index = 0;
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
        GUILayout.Label("Marks", EditorStyles.boldLabel);
        toolbar_index = GUILayout.Toolbar(toolbar_index, toolbars);

        GUILayout.Space(15);

        GUILayout.Label("Lanes", EditorStyles.boldLabel);
        lane_toolbar_index = GUILayout.Toolbar(lane_toolbar_index, lane_toolbars);


        Instance = this;
    }

    private void OnDisable()
    {
        toolbar_index = 0;
        lane_toolbar_index = 0;
    }
}
