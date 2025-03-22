using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Race
{
    public class RaceList : MonoBehaviour
    {
        [MenuItem("Game/Create Default Race")]
        public static void CreateRaceDatabase()
        {
            RaceDatabase database = Resources.Load<RaceDatabase>("Database/RaceDatabase");

            if (database == null)
            {
                Debug.LogError(("RaceDatabase를 찾을 수 없습니다. By.RaceList.cs"));
                return;
            }

            database.raceList = new List<RaceData>();
            
            
            
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("기본 종족 데이터를 추가했습니다");
        }
    }
}
