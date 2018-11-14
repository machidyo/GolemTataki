using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Linq;
using UniRx;
using Valve.VR;

public class MonsterSlayer : MonoBehaviour
{
    [Header("Input")]
    [SerializeField]
    private SteamVR_Input_Sources _handType;
    [SerializeField]
    private SteamVR_Action_Boolean _grabAction;

    [Header("Player")]
    [SerializeField]
    private GameObject _hand;

    [Header("Magic")]
    [SerializeField]
    private GameObject _thunder;
    [SerializeField]
    private GameObject _pyramid;

    private Ray _ray;
    private Plane _plane = new Plane();

    private bool isVR = false;

    void Start()
    {
        var tm = GetComponentInChildren<RFX4_TransformMotion>(true);
        if (tm != null) tm.CollisionEnter += Tm_CollisionEnter;

        if (isVR)
        {
            Observable
                .EveryUpdate()
                .Where(_ => SteamVR_Input._default.inActions.InteractUI.GetState(_handType))
                .ThrottleFrame(5)
                .Subscribe(_ => Fire("thunder"));

            Observable
                .EveryUpdate()
                .Where(_ => _grabAction.GetState(_handType))
                .ThrottleFrame(5)
                .Subscribe(_ => Fire("pyramid"));
        }
        else
        {
            Observable
                .EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .ThrottleFrame(5)
                .Subscribe(_ => Fire("thunder"));
        }
    }

    private void Update()
    {
        if (!isVR)
        {
            RotateSlayerWithoutVR();
        }
    }
    private void RotateSlayerWithoutVR()
    {
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);   
        _plane.SetNormalAndPosition(Vector3.up, transform.localPosition);

        var distance = 0.0f;
        if (_plane.Raycast(_ray, out distance))
        {
            var lookPoint = _ray.GetPoint(distance);
            transform.LookAt(lookPoint);
        }
    }

    public void Fire(string kind)
    {
        switch (kind)
        {
            case "thunder":
                if (isVR)
                {
                    Thunder(_thunder);
                }
                else
                {
                    FireWithoutVR(_thunder);
                }
                break;
            case "pyramid":
                Pyramid(_pyramid);
                break;
            default:
                break;
        }
    }
    private void FireWithoutVR(GameObject magic)
    {
        var pos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        // var rot = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, 0));
        var m = Instantiate(magic, pos, transform.rotation);
        var tm = m.GetComponentInChildren<RFX4_TransformMotion>(true);
        if (tm != null)
        {
            tm.CollisionEnter += Tm_CollisionEnter;
            // SPED: important!! if you not set active, this effect does NOT call Tm_CollisionEnter!!
            m.SetActive(true);
        }
        Destroy(m, 1);
    }

    private void Thunder(GameObject magic)
    {
        var m = Instantiate(magic, _hand.transform.position, _hand.transform.rotation);
        var tm = m.GetComponentInChildren<RFX4_TransformMotion>(true);
        if (tm != null)
        {
            tm.CollisionEnter += Tm_CollisionEnter;
            // SPED: important!! if you not set active, this effect does NOT call Tm_CollisionEnter!!
            m.SetActive(true);
        }
        Destroy(m, 1);
    }

    private void Pyramid(GameObject magic)
    {
        var m = Instantiate(magic, _hand.transform.position, _hand.transform.rotation * Quaternion.AngleAxis(90, Vector3.right));
        var tm = m.GetComponentInChildren<RFX4_TransformMotion>(true);
        if (tm != null)
        {
            tm.CollisionEnter += Tm_CollisionEnter;
            // SPED: important!! if you not set active, this effect does NOT call Tm_CollisionEnter!!
            m.SetActive(true);
        }
        Destroy(m, 3);
    }

    private void Tm_CollisionEnter(object sender, RFX4_TransformMotion.RFX4_CollisionInfo e)
    {
        Debug.Log("Magic hit a " + e.Hit.transform.name);

        // todo: IMonster とか tag とかで分けたい
        if (e.Hit.transform.name.ToLower().StartsWith("golem")
            || e.Hit.transform.name.ToLower().StartsWith("chimera"))
        {
            var direction = e.Hit.transform.position - transform.position;
            e.Hit.rigidbody.AddForce(direction.normalized * 600.0f, ForceMode.Force);
            e.Hit.transform.GetComponent<GolemController>().Death();
        }
    }
}
