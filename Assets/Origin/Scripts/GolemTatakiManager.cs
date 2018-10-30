using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GolemTatakiManager : MonoBehaviour
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
        var rand = Mathf.FloorToInt(UnityEngine.Random.Range(0.0f, 4.99f) % 4);
        var pos = _rocks[rand].transform.position;
        var tran = new Quaternion();

        var golem = Instantiate(_golem, pos, tran);
        golem.GetComponent<IMonster>().IsDead.Where(isDead => isDead).Subscribe(_ => KilledCount.Value += 1);
    }
}
