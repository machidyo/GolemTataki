using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _rocks;

    [Header("Monsters")]
    [SerializeField]
    private GameObject _golem;

    public ReactiveProperty<int> KilledCount = new ReactiveProperty<int>();

    void Start()
    {
        Observable
            .Interval(TimeSpan.FromMilliseconds(2000))
            .Subscribe(_ => GenerateGolem());

        KilledCount.Subscribe(c => Debug.Log(c));
    }

    void Update()
    {

    }

    private void GenerateGolem()
    {
        var gameManagerPos = transform.position;
        var x = gameManagerPos.x + UnityEngine.Random.Range(3, -3);
        var y = gameManagerPos.y + 10;
        var z = gameManagerPos.z + UnityEngine.Random.Range(3, -3);

        var pos = new Vector3(x, y, z);
        var tran = new Quaternion();

        var golem = Instantiate(_golem, pos, tran);
        golem.GetComponent<IMonster>().IsDead.Where(isDead => isDead).Subscribe(_ => KilledCount.Value += 1);
    }
}
