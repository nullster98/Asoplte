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

            #region 골렘 생성
            List<SubRaceData> golemSubRaces = new List<SubRaceData>
            {
                CreateRace("룬 골렘", "RuneGolem", 0, true),
                CreateRace("배터리 골렘", "LightGolem", 100, false),
                CreateRace("숲 골렘", "ForestGolem", 150, false),
            };
            #endregion
            #region 오크 생성

            List<SubRaceData> oakSubRaces = new List<SubRaceData>
            {
                CreateRace("오크 전사", "OakWarrior", 0, true),
                CreateRace("잿빛 오크", "GrayOak", 100, false),
                CreateRace("핏빛 오크", "RedOak", 150, false),
            };

            #endregion

            RaceData golem = CreateTribe("골렘", "Golem", golemSubRaces);
            RaceData oak = CreateTribe("오크", "Oak", oakSubRaces);
            
            database.raceList.Add(golem);
            database.raceList.Add(oak);
            
            
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("기본 종족 데이터를 추가했습니다");
        }
        
        //대종족 생성 함수
        private static RaceData CreateTribe(string tribeName, string fileName, List<SubRaceData> subRace)
        {
            RaceData newTribe = new RaceData
            {
                raceName = tribeName,
                fileName = fileName,
                subRace = new List <SubRaceData>(subRace)
            };

            return newTribe;
        }

        private static SubRaceData CreateRace(string raceName, string fileName, 
            int requireFaith, bool isUnlocked)
        {
            return new SubRaceData
            {
                subRaceName = raceName,
                fileName = fileName,
                requireFaith = requireFaith,
                isUnlocked = isUnlocked
            };
        }
    }
}
