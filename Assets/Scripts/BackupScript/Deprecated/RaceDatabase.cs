// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Race
// {
//     [CreateAssetMenu(fileName = "RaceDatabase", menuName = "Game/Race Database")]
//     public class RaceDatabase : ScriptableObject
//     {
//         public List<RaceData> raceList = new List<RaceData>();
//
//         public RaceData GetGodByIndex(int index)
//         {
//             if (index < 0 || index >= raceList.Count)
//             {
//                 Debug.LogError("인덱스 범위를 초과하였습니다.");
//                 return null;
//             }
//             return raceList[index];
//         }
//
//         public void ResetDatabase()
//         {
//             raceList.Clear();
//         }
//     }
// }
