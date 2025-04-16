// using System.Collections.Generic;
// using System.IO;
// using Event;
// using UnityEngine;
//
// public class JSONCreator : MonoBehaviour
// {
//     [ContextMenu("이벤트 JSON 생성")]
//     public void CreateEventJSON()
//     {
//         EventData exampleEvent = new EventData
//         {
//             eventName = "시작이벤트",
//             eventType = EventTag.None,
//             phases = new List<EventPhase>
//             {
//                 new EventPhase
//                 {
//                     phaseName = "Phase1",
//                     phaseOutcome = new EventOutcome
//                     {
//                         spawnEntity = false,
//                         rewardTrigger = false
//                     },
//                     dialogues = new List<DialogueBlock>
//                     {
//                         new DialogueBlock
//                         {
//                             dialogueText = "앞에 누군가 있다.",
//                             choices = new List<EventChoice>
//                             {
//                                 new EventChoice
//                                 {
//                                     choiceName = "다가간다",
//                                     nextPhaseIndex = 1
//                                 },
//                                 new EventChoice
//                                 {
//                                     choiceName = "무시한다",
//                                     nextEventName = "다음이벤트"
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
//         };
//
//         // JSON 문자열로 변환 (Pretty Print)
//         string json = JsonUtility.ToJson(exampleEvent, true);
//
//         // 저장 경로 설정 (Unity Editor 전용 Resources 폴더 기준)
//         string path = Application.dataPath + "/Resources/Event/start_event.json";
//
//         File.WriteAllText(path, json);
//         Debug.Log("이벤트 JSON 생성 완료: " + path);
//     }
// }
