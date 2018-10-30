using System.Collections;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Animator))]
public class GolemController : MonoBehaviour, IMonster
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

        Observable
            .EveryUpdate()
            .Where(_ => Input.anyKeyDown)
            .Subscribe(_ => ActionByKey());
    }

    void Update()
    {
        if(_isWalking)
        {
            transform.LookAt(_target.transform.position);
            transform.position = Vector3.Lerp(gameObject.transform.position, _target.transform.position, Time.deltaTime / 10);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.ToLower().StartsWith("rock"))
        {
            _isWalking = false;

            Observable.FromCoroutine(Attack).Subscribe(_ => 
            {
                gameObject.transform.position = _startPoint;
                IsGrounded = _startPoint.y <= 0.3; // 0.3 がおおむね足が接地する高さ
            });
        }

        if (!IsGrounded && collision.gameObject.name.ToLower().StartsWith("terrain"))
        {
            IsGrounded = true;

            Observable.FromCoroutine(Roar).Subscribe(_ =>
            {
                _isWalking = true;
                StartCoroutine(Walk());
            });
        }
    }

    public IEnumerator Walk()
    {
        return PlayAnimation("walk");
    }

    public IEnumerator Roar()
    {
        return PlayAnimation("roar");
    }

    public IEnumerator Attack()
    {
        return PlayAnimationOneTime("attack1");
    }

    public void Death()
    {
        _isWalking = false;
        IsDead.Value = true;
        StartCoroutine(AnimateDeath());
    }

    private IEnumerator AnimateDeath()
    {
        yield return PlayAnimation("death");

        Destroy(gameObject);
    }

    private void ActionByKey()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _animator.Play("3HitComboForward_RM");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _animator.Play("attack1");
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _animator.Play("turn90Right");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _animator.Play("turn90Left");
        }
    }

    private IEnumerator PlayAnimation(string animationName)
    {
        _animator.Play(animationName);

        yield return null; // ここでいったん戻して animation を設定する

        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
    }

    private IEnumerator PlayAnimationOneTime(string animationName)
    {
        yield return PlayAnimation(animationName);

        _animator.Play("idle");
    }
}
