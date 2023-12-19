using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public float hittimer;
    public int hp=100;
    public bool blocking;
    public GameObject blood;
    public int faction;
    protected virtual void Start()
    {
        EntityManager.only.Add(this);
    }
    protected void EntityUpdate() 
    {
        hittimer -= Time.deltaTime;
        if (hp <= 0) { Die(); }
    }
    protected virtual void Die() 
    {
        Debug.Log("DIE NOT IMPLEMENTED");
        EntityManager.only.Die(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hittimer > 0||blocking) return;
        if (other.transform.CompareTag("Weapon")&&other.transform.GetComponent<Weapon>().activated) 
        {
            //Debug.Log("HIT" + transform.name);
            hp -= other.transform.GetComponent<Weapon>().dmg;
            Destroy(Instantiate(blood, other.transform.position,Quaternion.identity), 1f);
            hittimer = .1f;
        }
    }
}
