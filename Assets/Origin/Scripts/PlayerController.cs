using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerController : MonoBehaviour
{
    public GameObject CharacterEffect;
    public Transform CharacterAttachPoint;
    public GameObject Effect;
    public Transform AttachPoint;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private GameObject _target;

    private Ray _ray;
    private Plane _plane = new Plane();

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

    private void OnEnable()
    {
        if (Effect != null)
        {
            Effect.SetActive(false);
        }
        if (CharacterEffect != null)
        {
            CharacterEffect.SetActive(false);
        }
    }

    private void Update()
    {
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        _plane.SetNormalAndPosition(Vector3.up, transform.localPosition);

        var distance = 0.0f;
        if(_plane.Raycast(_ray, out distance))
        {
            var lookPoint = _ray.GetPoint(distance);
            transform.LookAt(lookPoint);
        }
    }

    void LateUpdate()
    {
        if (Effect != null && AttachPoint != null)
        {
            Effect.transform.position = AttachPoint.position;
        }
        if (CharacterEffect != null && CharacterAttachPoint != null)
        {
            CharacterEffect.transform.position = CharacterAttachPoint.position;
        }
    }

    public void ActivateEffect()
    {
        if (Effect == null) return;
        Effect.SetActive(true);
    }

    public void ActivateCharacterEffect()
    {
        if (CharacterEffect == null) return;
        CharacterEffect.SetActive(true);
    }

    public void ActivateCharacterEffect2()
    {
        // nothing to do, but this function is called from Jump.
    }

    public void OnAfterAttack1()
    {
        _animator.SetBool("Thunder", false);
    }

    public void Fire()
    {
        _animator.SetBool("Thunder", true);
    }

    private void Tm_CollisionEnter(object sender, RFX4_TransformMotion.RFX4_CollisionInfo e)
    {
        // Debug.Log(e.Hit.transform.name); //will print collided object name to the console.

        var direction = e.Hit.transform.position - transform.position;
        e.Hit.rigidbody.AddForce(direction.normalized * 600.0f, ForceMode.Force);
        e.Hit.transform.GetComponent<GolemController>().Death();
    }
}
