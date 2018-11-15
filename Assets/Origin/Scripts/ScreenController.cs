using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ScreenController : MonoBehaviour
{
    [SerializeField] GolemTatakiManager _golemTatakiManager;

    [Header("UI")]
    [SerializeField] Text _timerText;
    [SerializeField] Text _scoreText;
    [SerializeField] Text _mainMessage;
    [SerializeField] GameObject _startButton;

    void Start()
    {
        _golemTatakiManager.Status.Subscribe(s =>
        {
            switch (s)
            {
                case GolemTatakiManager.GameStatus.Idle:
                    _timerText.enabled = false;
                    _scoreText.enabled = false;
                    _startButton.SetActive(true);
                    _mainMessage.fontSize = 60;
                    _mainMessage.color = Color.yellow;
                    _mainMessage.text = "Please click button to start";
                    break;
                case GolemTatakiManager.GameStatus.Start:
                    _timerText.enabled = true;
                    _scoreText.enabled = true;
                    _startButton.SetActive(false);
                    break;
                case GolemTatakiManager.GameStatus.Finished:
                    _timerText.enabled = false;
                    _scoreText.enabled = false;
                    _startButton.SetActive(true);
                    _mainMessage.fontSize = 60;
                    _mainMessage.color = Color.yellow;
                    _mainMessage.text = "You killed " + _golemTatakiManager.KilledCount.Value;
                    break;
                case GolemTatakiManager.GameStatus.Playing:
                default:
                    break;
            }
        });

        _golemTatakiManager.TimeCount.Subscribe(t => _timerText.text = "Timer: " + t);
        _golemTatakiManager.KilledCount.Subscribe(c => _scoreText.text = "Score: " + c);
    }

    public void OnStartClick()
    {
        _startButton.SetActive(false);

        Observable
            .Interval(TimeSpan.FromSeconds(1))
            .Take(5)
            .Subscribe(time =>
            {
                // Debug.Log("time is " + time);

                string count;
                switch(Mathf.FloorToInt(time))
                {
                    case 0:
                        count = "③";
                        break;
                    case 1:
                        count = "②";
                        break;
                    case 2:
                        count = "①";
                        break;
                    case 3:
                        count = "START!!";
                        break;
                    default:
                        count = "";
                        break;
                }
                _mainMessage.fontSize = 60;
                _mainMessage.color = Color.yellow;
                _mainMessage.text = count;

                if (time >= 4)
                {
                    _golemTatakiManager.StartGame();                   
                }
            });
    }

    // http://inter-high-blog.unity3d.jp/2017/08/22/resolutionchange/
    // 画面サイズが前回実行時を覚えている多ときに初期設定が効かないクソ仕様対応
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        Screen.SetResolution(1365, 768, false);
    }
}
