using System.Collections.Generic;
using UnityEngine;

namespace Trait
{
    [CreateAssetMenu(fileName = "TraitDatabase", menuName = "Game/Trait Database")]
    public class TraitDatabase : ScriptableObject
    {
        public List<TraitData> traitList = new List<TraitData>();

        public TraitData GetTraitByID(int traitID)
        {
            return traitList.Find(trait => trait.traitID == traitID);
        }

        public void ResetDatabase()
        {
            traitList.Clear();
        }
    }
}
