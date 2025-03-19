using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    public ItemDatabase itemDatabase;
    public EventDatabase eventDatabase;
    public EnemyDatabase enemyDatabase;
    //public TraitDatabae traitDatabase;
    //public SkillDatabase skillDatabase;
    //public RaceDatabse raceDatabase;
    //public GodDatabase godDatabase;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
