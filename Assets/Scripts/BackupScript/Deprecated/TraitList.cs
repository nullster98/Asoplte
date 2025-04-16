using Game;
using UnityEditor;
using UnityEngine;

namespace Trait
{
   /* public class TraitList : MonoBehaviour
    {
        [MenuItem("Game/Create All Traits")]
        public static void CreateAllTraits()
        {
            TraitDatabase db = GetTraitDatabaseEditorSafe();
            if (db == null) return;

            // 🔥 기존 특성 초기화
            db.ResetDatabase(); // traitList.Clear() 내부에서 처리
               
            Debug.Log("모든 특성 생성 시작...");
            #region Positive
            AddTraitToDatabase(new TraitData("굳건한 신앙", "StrongFaith", TraitPnN.Positive, 1001001,5, true,new StrongFaith()));
            AddTraitToDatabase(new TraitData("폭발적인 힘", "PowerfulForce", TraitPnN.Positive, 1001002,7, true,null));
            AddTraitToDatabase(new TraitData("정신감응", "MentalResponse", TraitPnN.Positive, 1001003,8, true,null));
            AddTraitToDatabase(new TraitData("절대자", "Absolute", TraitPnN.Positive, 1001999,9999, false,null));
            #endregion
            #region Negative
            AddTraitToDatabase(new TraitData("맹인", "Blind", TraitPnN.Negative, 1005001,10, true,null));
            AddTraitToDatabase(new TraitData("허약", "Weakness", TraitPnN.Negative, 1005002,5, true,null));
            AddTraitToDatabase(new TraitData("불면", "Sleepless", TraitPnN.Negative, 1005003,7, true,null));
            AddTraitToDatabase(new TraitData("절대자", "Absolute", TraitPnN.Negative, 1005999,9999, true,null));
            #endregion
            Debug.Log("모든 특성 생성 완료!");
        }


        private static void AddTraitToDatabase(TraitData traitdata)
        {
            TraitDatabase db = GetTraitDatabaseEditorSafe();
            if (db != null)
            {
                db.traitList.Add(traitdata);
                EditorUtility.SetDirty(db);
                AssetDatabase.SaveAssets();
                Debug.Log($"{traitdata.traitName}이(가) 데이터베이스에 추가되었습니다");
            }

            else
            {
                Debug.LogError("TraitDatabase를 찾을 수 없습니다! Resources 폴더 내 경로를 확인하세요.");
            }
        }
        
#if UNITY_EDITOR
        private static TraitDatabase GetTraitDatabaseEditorSafe()
        {
            var db = DatabaseManager.Instance?.traitDatabase;

            if (db == null)
            {
                db = Resources.Load<TraitDatabase>("Database/TraitDatabase");
                if (db == null)
                {
                    Debug.LogError("TraitDatabase를 Resources/Database 경로에서 찾을 수 없습니다!");
                }
            }

            return db;
        }
#endif
    }*/
}
