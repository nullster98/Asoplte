using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRemovableEffect : IEffect
{
    void RemoveEffect(IUnit target);
}
