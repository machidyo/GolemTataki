using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Gun : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField]
    private GameObject _bullet;
    [SerializeField]
    private float _bulletSpeed = 100f;

    [Header("Magic")]
    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private GameObject _magic;

    void Start()
    {
        var tm = GetComponentInChildren<RFX4_TransformMotion>(true);
        if (tm != null) tm.CollisionEnter += Tm_CollisionEnter;

        Observable
            .EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .ThrottleFrame(5)
            .Subscribe(_ => Fire());
    }

    void Fire()
    {
        // for bullet
        // var centerOfLens = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane + 0.5f));
        // var bullet = Instantiate(_bullet, centerOfLens, Quaternion.identity) as GameObject;
        // bullet.GetComponent<Bullet>().Speed = _bulletSpeed;

        // for magic
        var pos = new Vector3(_player.transform.position.x, 2, _player.transform.position.z);
        var magic = Instantiate(_magic, pos, Quaternion.identity) as GameObject;
        Destroy(magic, 3);
    }

    private void Tm_CollisionEnter(object sender, RFX4_TransformMotion.RFX4_CollisionInfo e)
    {
        var direction = e.Hit.transform.position - transform.position;
        e.Hit.rigidbody.AddForce(direction.normalized * 600.0f, ForceMode.Force);
        e.Hit.transform.GetComponent<GolemController>().Death();
    }
}
