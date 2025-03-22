using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace God
{
    public class GodList : MonoBehaviour
    {
        [MenuItem("Game/Create Default Gods")]
        public static void CreateGodDatabase()
        {
            // 기존 GodDatabase.asset을 찾기
            GodDatabase database = Resources.Load<GodDatabase>("Database/GodDatabase");

            if (database == null)
            {
               Debug.LogError("GodDatabase를 찾을수 업습니다.By.GodList.cs");
               return;
            }

            database.godList = new List<GodData>();
            
            // 신 데이터 추가
            database.godList.Add(new GodData("자유의 신", "Liberty", 1, true, 0, new LibertyGodEffect()));
            database.godList.Add(new GodData("복수의 신", "Revenge", 2, true, 0, new LibertyGodEffect()));
            database.godList.Add(new GodData("쾌락의 신 ", "Furious", 3, true, 0, new LibertyGodEffect()));
            
            // 변경 사항 저장
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("기본 신 데이터를 추가했습니다!");
        }
        
    }
}
