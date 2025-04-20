using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IEffect를 상속한 인터페이스, 해제가 가능한 효과를 정의
public interface IRemovableEffect : IEffect
{
    void RemoveEffect(IUnit target);
}
