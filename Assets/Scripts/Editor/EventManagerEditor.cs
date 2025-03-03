using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(EventManager))]
//[CanEditMultipleObjects]
public class EventManagerEditor : Editor
{
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        EventManager manager = (EventManager)target;

        // `Events` ����Ʈ�� ã��
        reorderableList = new ReorderableList(serializedObject,
            serializedObject.FindProperty("Events"), // ����ȭ�� ����Ʈ ã��
            true, true, true, true);

        // ����Ʈ ��� ����
        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "�̺�Ʈ ����Ʈ (�巡���Ͽ� ���� ���� ����)");
        };  

        // ����Ʈ ��� UI ����
        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element, GUIContent.none);
        };

        // ��� �̵� �� ����
        reorderableList.onReorderCallback = (ReorderableList list) =>
        {
            Debug.Log("�̺�Ʈ ����Ʈ ���� �����!");
        };
    }

    public override void OnInspectorGUI()
    {
        //serializedObject.Update();

        EventManager manager = (EventManager)target;

        DrawDefaultInspector(); // �⺻ �ν����� UI ����

        //reorderableList.DoLayoutList();

        //  Getter�� ����ؼ� �̺�Ʈ ����Ʈ ��������
        List<EventData> events = manager.GetEvents();

        if (events == null || events.Count == 0)
        {
            EditorGUILayout.HelpBox("�̺�Ʈ ����Ʈ�� ��� �ֽ��ϴ�!", MessageType.Warning);
            return;
        }

        foreach (var evt in events)
        {
            if (evt.Buttons == null) continue;

            foreach (var choice in evt.Buttons)
            {
                EditorGUILayout.LabelField($"������: {choice.ChoiceName}");

                //  ��Ӵٿ� ����Ʈ�� "���� �̺�Ʈ ����" �ɼ� �߰�
                List<string> eventNames = new List<string> { "���� �̺�Ʈ ����" }; // ù ��° �ɼ� �߰�
                eventNames.AddRange(events.ConvertAll(e => e.EventName));

                // ���� ���õ� �̺�Ʈ ID ��������
                int currentIndex = choice.NextEventID + 1;

                // ��Ӵٿ� UI ǥ��
                int selectedIndex = EditorGUILayout.Popup("���� �̺�Ʈ", currentIndex, eventNames.ToArray());

                // ���õ� �̺�Ʈ ����
                if (selectedIndex == 0)
                {
                    choice.NextEventID = -1; //  ID�� -1�� ����
                }
                else
                {
                    choice.NextEventID = selectedIndex - 1;
                }
            }
        }

        //serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(manager);
        }
    }
}
