using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float Speed = 100f;

    private Rigidbody _rigidbody;
    private Ray _ray;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _rigidbody.AddForce(_ray.direction * Speed, ForceMode.Force);
    }

    private void OnEnable()
    {
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Destroy(gameObject, 3);
    }
}
