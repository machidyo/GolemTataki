using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public interface IMonster
{
    ReactiveProperty<bool> IsDead { get; }

    IEnumerator Walk();
    IEnumerator Attack();
    void Death();
}
