using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GolemTatakiManager : MonoBehaviour
{
    public enum GameStatus
    {
        Idle,
        Start,
        Playing,
        Finished,
    }

    [SerializeField]
    private List<GameObject> _rocks;

    [SerializeField]
    private GameObject _target;

    [Header("Monsters")]
    [SerializeField]
    private GameObject _golem;

    public ReactiveProperty<GameStatus> Status { get; private set; }
    public ReactiveProperty<int> TimeCount  { get; private set; }
    public ReactiveProperty<int> KilledCount { get; private set; }

    private int _playTime = 10;
    private IDisposable _timer;
    private IDisposable _generater;
    private List<GameObject> _monsters = new List<GameObject>();

    private void OnEnable()
    {
        // SPEC: This is game master, so init in on enable.
        Status = new ReactiveProperty<GameStatus>();
        TimeCount = new ReactiveProperty<int>();
        KilledCount = new ReactiveProperty<int>();
    }

    void Start()
    {
        Status.DistinctUntilChanged().Subscribe(s =>
        {
            switch(s)
            {
                case GameStatus.Idle:
                    break;
                case GameStatus.Start:
                    InitGame();
                    break;
                case GameStatus.Playing:
                    break;
                case GameStatus.Finished:
                    FinishGame();
                    break;
            }

        });

        TimeCount.Subscribe(x => Debug.Log(x));
        KilledCount.Subscribe(x => Debug.Log(x));
    }

    /// <summary>
    /// for call from outside class.
    /// </summary>
    public void StartGame()
    {
        Status.Value = GameStatus.Start;
    }

    private void InitGame()
    {
        TimeCount.Value = _playTime;
        KilledCount.Value = 0;

        _timer = Observable
            .Interval(TimeSpan.FromMilliseconds(1000))
            .Subscribe(_ =>
            {
                TimeCount.Value--;
                if (TimeCount.Value <= 0)
                {
                    Status.Value = GameStatus.Finished;
                }
            });

        _generater = Observable
            .Interval(TimeSpan.FromMilliseconds(2000))
            .Subscribe(_ => GenerateGolem());
    }
    private void GenerateGolem()
    {
        var rand = Mathf.FloorToInt(UnityEngine.Random.Range(0.0f, 4.99f) % 4);
        var pos = _rocks[rand].transform.position;
        var tran = new Quaternion();

        var golem = Instantiate(_golem, pos, tran);
        golem.GetComponent<IMonster>().IsDead.Where(isDead => isDead).Where(_ => Status.Value != GameStatus.Finished).Subscribe(_ => KilledCount.Value += 1);
        golem.GetComponent<GolemController>().SetTarget(_target);
        _monsters.Add(golem);
    }

    private void FinishGame()
    {
        _timer.Dispose();
        _generater.Dispose();

        foreach (var m in _monsters)
        {
            m.GetComponent<IMonster>().Death();
        }
        _monsters.Clear();
    }
}
