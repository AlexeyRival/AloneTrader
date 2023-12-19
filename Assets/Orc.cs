using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : Entity
{
    public Animator animator;
    public Entity target;
    public orcState state;
    public float actionTimer;
    public Vector3 targetPoint;
    public NavChunk navigator;
    private RaycastHit hit;
    private float lastcontact = 0;
    private Rigidbody rb;
    public Weapon weapon;
    public Renderer mark;
    private float totaltimer;
    // Update is called once per frame
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        mark.material.SetColor("_Color",Color.HSVToRGB(Mathf.Sin(faction*.01f),0.8f,0.8f));
    }
    void Update()
    {
        EntityUpdate();
        actionTimer += Time.deltaTime;
        switch (state)
        {
            case orcState.idle:
                if (target)
                {
                    if (actionTimer > 1)
                    {
                        if (Vector3.Distance(transform.position, target.transform.position) > 4)
                        {
                            //    targetPoint = target.transform.position;
                            targetPoint = navigator.GetNearestPoint(target.transform.position);
                            SetState(orcState.walk);
                            lastcontact = 100f;
                            break;
                        }
                        transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                        orcState next = orcState.hit;
                        switch (Random.Range(0, 6))
                        {
                            case 0: next = orcState.hit; break;
                            case 1: next = orcState.hit; break;
                            case 2: next = orcState.defend; break;
                            case 3: next = orcState.defend; break;
                            case 4: next = orcState.maneur; break;
                            case 5: next = orcState.hit; break;
                        }
                        SetState(next);
                    }

                }
                else 
                {
                    if (actionTimer > 2)
                    {
                        target = EntityManager.only.GetNearEnemy(transform.position, faction);
                        actionTimer = 0;
                    }
                }
                break;
            case orcState.sit:
                if (actionTimer > 2)
                {
                    target = EntityManager.only.GetNearEnemy(transform.position, faction);
                    if (target)//Vector3.Distance(transform.position, target.transform.position) < 10)
                    {
                        SetState(orcState.walk);
                        targetPoint = transform.position + transform.forward * 1;
                    }
                    actionTimer = 0;
                }
                break;
            case orcState.walk:
                totaltimer += Time.deltaTime;
                if (actionTimer > 1f)
                {
                    if (target && (transform.position - target.transform.position).sqrMagnitude < 5f&&lastcontact>10) 
                    {
                        SetState(orcState.idle);
                        lastcontact = 0;
                        break;
                    }
                    if (totaltimer >= 30) { SetState(orcState.idle);break; }
                    transform.LookAt(new Vector3(targetPoint.x, transform.position.y, targetPoint.z));
                    transform.Translate(0, 0, 1.5f * Time.deltaTime);
                    if (actionTimer > 10&&targetPoint.y>transform.position.y+2) { rb.AddRelativeForce(0, 10, 0,ForceMode.Impulse); actionTimer = 1; }
                    Debug.DrawLine(transform.position, targetPoint);
                    if (Physics.Raycast(transform.position + transform.up + transform.forward, -transform.up, out hit, 1f))
                    {
                        transform.Translate(0, Time.deltaTime * 1.5f, Time.deltaTime * 2);
                    }
                    if (Vector3.Distance(transform.position, targetPoint) < Mathf.Max(1, actionTimer * .125f))
                    {
                        lastcontact = 10;
                        SetState(orcState.idle);
                    }
                }
                break;
            case orcState.defend:
                if (!target) { SetState(orcState.idle);break; }
                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                if (actionTimer > 5f && Random.Range(0, 5) == 0) { SetState(orcState.idle); }
                break;
            case orcState.hit:
                if (actionTimer > 1f) { SetState(orcState.idle); }
                break;
            case orcState.maneur:
                targetPoint = navigator.GetNearestPoint(transform.position) + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                Debug.DrawLine(transform.position, targetPoint, Color.green, 1f);
                lastcontact -= 10f;
                SetState(orcState.walk);
                actionTimer = 1;
                break;
        }
    }
    protected override void Die()
    {
        Debug.Log("ORC DIE");
        EntityManager.only.Die(this);
        this.enabled = false;
        animator.enabled = false;
        DropAll();
        Destroy(gameObject,5f);
    }
    #region ragdoll
    private void DropAll()
    {
        DropChild(transform.GetChild(0));
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().mass = 10f;
        GetComponent<Rigidbody>().useGravity = true;

        GetComponent<Rigidbody>().AddRelativeForce(0, 50f, 0, ForceMode.Impulse);
    }
    private void DropChild(Transform trans)
    {
        if (trans.GetComponent<CapsuleCollider>())
        {
            trans.GetComponent<CapsuleCollider>().enabled = true;
            if (!trans.GetComponent<Rigidbody>())
            {
                trans.gameObject.AddComponent<Rigidbody>();
                trans.gameObject.GetComponent<Rigidbody>().mass = GetComponent<Rigidbody>().mass;
                trans.gameObject.GetComponent<Rigidbody>().AddForce(0, 5f, 0, ForceMode.Impulse);
                if (trans.parent && trans.parent.GetComponent<Rigidbody>())
                {
                    trans.gameObject.AddComponent<CharacterJoint>();
                    trans.gameObject.GetComponent<CharacterJoint>().connectedBody = trans.parent.GetComponent<Rigidbody>();
                    trans.gameObject.GetComponent<CharacterJoint>().autoConfigureConnectedAnchor = true;
                }

            }
            else
            {
                trans.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                trans.GetComponent<Rigidbody>().useGravity = true;
            }
        }
        if (trans.childCount == 0) { return; }
        for (int i = 0; i < trans.childCount; ++i)
        {
            DropChild(trans.GetChild(i));
        }
    }
    #endregion
    public void SetState(orcState newstate)
    {
        actionTimer = 0;
        switch (state)
        {
            case orcState.idle:
                break;
            case orcState.sit:
                animator.SetTrigger("standing");
                break;
            case orcState.walk:
                animator.SetBool("walk", false);
                break;
            case orcState.defend:
                animator.SetBool("defend", false);
                blocking = false;
                break;
            case orcState.hit:
                weapon.activated = false;
                break;
            case orcState.maneur:
                break;
        }
        switch (newstate)
        {
            case orcState.idle:
                break;
            case orcState.sit:
                break;
            case orcState.walk:
                animator.SetBool("walk", true);
                animator.SetTrigger("goWalk");
                totaltimer = 0;
                break;
            case orcState.defend:
                animator.SetBool("defend", true);
                animator.SetTrigger("block");
                blocking = true;
                break;
            case orcState.hit:
                animator.SetTrigger("hit");
                weapon.activated = true;
                break;
            case orcState.maneur:
                break;
        }
        state = newstate;
    }
    public enum orcState 
    {
        idle,
        sit,
        walk,
        defend,
        hit,
        maneur
    }
}
