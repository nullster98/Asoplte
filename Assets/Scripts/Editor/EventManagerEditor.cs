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

        // `Events` 리스트를 찾기
        reorderableList = new ReorderableList(serializedObject,
            serializedObject.FindProperty("Events"), // 직렬화된 리스트 찾기
            true, true, true, true);

        // 리스트 헤더 설정
        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "이벤트 리스트 (드래그하여 순서 변경 가능)");
        };  

        // 리스트 요소 UI 설정
        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element, GUIContent.none);
        };

        // 요소 이동 시 동작
        reorderableList.onReorderCallback = (ReorderableList list) =>
        {
            Debug.Log("이벤트 리스트 순서 변경됨!");
        };
    }

    public override void OnInspectorGUI()
    {
        //serializedObject.Update();

        EventManager manager = (EventManager)target;

        DrawDefaultInspector(); // 기본 인스펙터 UI 유지

        //reorderableList.DoLayoutList();

        //  Getter를 사용해서 이벤트 리스트 가져오기
        List<EventData> events = manager.GetEvents();

        if (events == null || events.Count == 0)
        {
            EditorGUILayout.HelpBox("이벤트 리스트가 비어 있습니다!", MessageType.Warning);
            return;
        }

        foreach (var evt in events)
        {
            if (evt.Buttons == null) continue;

            foreach (var choice in evt.Buttons)
            {
                EditorGUILayout.LabelField($"선택지: {choice.ChoiceName}");

                //  드롭다운 리스트에 "랜덤 이벤트 실행" 옵션 추가
                List<string> eventNames = new List<string> { "랜덤 이벤트 실행" }; // 첫 번째 옵션 추가
                eventNames.AddRange(events.ConvertAll(e => e.EventName));

                // 현재 선택된 이벤트 ID 가져오기
                int currentIndex = choice.NextEventID + 1;

                // 드롭다운 UI 표시
                int selectedIndex = EditorGUILayout.Popup("다음 이벤트", currentIndex, eventNames.ToArray());

                // 선택된 이벤트 적용
                if (selectedIndex == 0)
                {
                    choice.NextEventID = -1; //  ID도 -1로 설정
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
