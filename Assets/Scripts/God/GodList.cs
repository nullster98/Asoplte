using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.iOS;
using UnityEngine;

public class GodList : MonoBehaviour
{
    [MenuItem("Game/Create Default Gods")]
    public static void CreateGodDatabase()
    {
        // 기존 GodDatabase.asset을 찾기
        GodDatabase database = Resources.Load<GodDatabase>("Database/GodDatabase");

        // 만약 없으면 새로 생성
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<GodDatabase>();

            // 경로에 새로운 데이터베이스 저장
            AssetDatabase.CreateAsset(database, "Assets/Resources/Database/GodDatabase.asset");
            AssetDatabase.SaveAssets();

            Debug.Log("새로운 GodDatabase.asset을 생성했습니다!");
        }

        // 신 데이터 추가

        // 변경 사항 저장
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log("기본 신 데이터를 추가했습니다!");
    }

    public static void CreateGod(int id, string name)
    {

    }

    public static Sprite LoadGodSprite(string imageName)
    {
        Sprite itemSprite = Resources.Load<Sprite>($"God/Images/{imageName}");
        if(itemSprite == null)
        {
            itemSprite = Resources.Load<Sprite>("images/default");
        }

        return itemSprite;
    }
}
