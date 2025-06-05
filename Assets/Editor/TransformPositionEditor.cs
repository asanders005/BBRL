using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Vector3))]
public class TransformPositionPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw label  
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Calculate rects with added vertical space  
        Rect objectFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        //Rect vector3Rect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 10, position.width, EditorGUIUtility.singleLineHeight); // Increased spacing

        // Draw ObjectField for GameObject  
        GameObject targetGameObject = (GameObject)EditorGUI.ObjectField(objectFieldRect, new GUIContent(property.vector3Value.ToString()), null, typeof(GameObject), true);

        // If a GameObject is selected, update the Vector3 property with its position  
        if (targetGameObject != null)
        {
            property.vector3Value = targetGameObject.transform.position;
        }

        // Draw the stored Vector3 below the ObjectField  
        //EditorGUI.LabelField(vector3Rect, "Stored Position", property.vector3Value.ToString());

        EditorGUI.EndProperty();
    }
}

[CustomEditor(typeof(CombatEncounterData))]
public class TransformPositionArray : Editor
{
    private SerializedObject obj;

    public void OnEnable()
    {
        obj = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        DropAreaGUI();
    }

    public void DropAreaGUI()
    {
        UnityEngine.Event evt = UnityEngine.Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, "Add Bricks");

        SerializedProperty brickLocations = obj.FindProperty("brickLocations");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                {
                    return;
                }

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object dragged_object in DragAndDrop.objectReferences)
                    {
                        brickLocations.arraySize++;
                        SerializedProperty newElement = brickLocations.GetArrayElementAtIndex(brickLocations.arraySize - 1);
                        if (dragged_object is GameObject gameObject)
                        {
                            // Assuming the GameObject has a Transform component
                            newElement.vector3Value = gameObject.transform.position;
                        }
                        else if (dragged_object is Transform transform)
                        {
                            newElement.vector3Value = transform.position;
                        }
                        else
                        {
                            Debug.LogWarning("Unsupported object type: " + dragged_object.GetType());
                        }
                    }

                    obj.ApplyModifiedProperties();
                }
                break;
        }
    }
}
