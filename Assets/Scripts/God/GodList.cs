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
        // ���� GodDatabase.asset�� ã��
        GodDatabase database = Resources.Load<GodDatabase>("Database/GodDatabase");

        // ���� ������ ���� ����
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<GodDatabase>();

            // ��ο� ���ο� �����ͺ��̽� ����
            AssetDatabase.CreateAsset(database, "Assets/Resources/Database/GodDatabase.asset");
            AssetDatabase.SaveAssets();

            Debug.Log("���ο� GodDatabase.asset�� �����߽��ϴ�!");
        }

        // �� ������ �߰�

        // ���� ���� ����
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log("�⺻ �� �����͸� �߰��߽��ϴ�!");
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
