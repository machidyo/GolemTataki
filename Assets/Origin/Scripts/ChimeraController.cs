using System;
using System.Collections;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Animator))]
public class ChimeraController : MonoBehaviour, IMonster
{
    [SerializeField]
    private GameObject _target;

    public bool IsGrounded = false;

    private Animator _animator;
    private Vector3 _startPoint;

    private bool _isWalking = false;
    public ReactiveProperty<bool> IsDead { get; } = new ReactiveProperty<bool>(false);

    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        _startPoint = transform.position;
    }

    void Update()
    {
        if(_isWalking)
        {
            transform.LookAt(_target.transform.position);
            transform.position = Vector3.Lerp(gameObject.transform.position, _target.transform.position, Time.deltaTime / 3);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!IsGrounded && collision.gameObject.name.ToLower().StartsWith("terrain"))
        {
            IsGrounded = true;

            StartCoroutine(Walk());
        }
    }

    public IEnumerator Walk()
    {
        Debug.Log("Walk");

        yield return null; // IMonster の IF に合わせるためだけの処置

        _isWalking = true;
        _animator.SetBool("IsWalking", true);
    }

    public IEnumerator Attack()
    {
        throw new NotImplementedException();
    }

    public void Death()
    {
        Debug.Log("Death");

        _isWalking = false;
        _animator.SetBool("IsWalking", false);

        IsDead.Value = true;
        _animator.SetBool("IsDead", true);

        Destroy(gameObject, 3);
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }
}
