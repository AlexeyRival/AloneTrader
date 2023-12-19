using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavChunk : MonoBehaviour
{
    public NavPoint[] navPoints;
    public bool DrawConnections;
    public bool recalculateConnections;
    public bool getChilds;
    private void OnDrawGizmos()
    {
        if(DrawConnections)for (int i = 0; i < navPoints.Length; ++i) 
        {
            for (int j = 0; j < navPoints[i].navPoints.Length; ++j)
            {
                Gizmos.DrawLine(navPoints[i].transform.position, navPoints[i].navPoints[j].transform.position);
            }
        }
        if (recalculateConnections) 
        {
            recalculateConnections = false;
            Recalc();
        }
        if (getChilds) 
        {
            getChilds = false;
            GetChilds();
        }
    }
    private void GetChilds() 
    {
        List<NavPoint> navs = new List<NavPoint>(transform.childCount);
        for (int i = 0; i < transform.childCount; ++i) 
        {
            navs.Add(transform.GetChild(i).GetComponent<NavPoint>());
        }
        navPoints = navs.ToArray();
    }
    private void Recalc()
    {
        Debug.Log($"Recalculating {navPoints.Length} navpoints");
        List<NavPoint> buffer;
        for (int i = 0; i < navPoints.Length; ++i)
        {
            buffer = new List<NavPoint>(navPoints[i].navPoints.Length);
            for (int j = 0; j < navPoints.Length; ++j)if(i!=j)
            {
                if (Mathf.Abs(navPoints[i].transform.position.y-navPoints[j].transform.position.y)<2
                        && Vector3.Distance(navPoints[i].transform.position,navPoints[j].transform.position)<6
                        &&!Physics.Raycast(navPoints[i].transform.position, navPoints[j].transform.position - navPoints[i].transform.position,Vector3.Distance(navPoints[i].transform.position,navPoints[j].transform.position)))
                {
                    buffer.Add(navPoints[j]);
                }
            }
            navPoints[i].navPoints = buffer.ToArray();
        }
        Debug.Log("Nav Recalculated");
    }

    public Vector3 GetNearestPoint(Vector3 currentposition) 
    {
        int minid=-1;
        float minvalue = 9999;
        float mag;
        for (int i = 0; i < navPoints.Length; ++i) 
        {
            mag = (navPoints[i].transform.position - currentposition).sqrMagnitude;
            if (mag < minvalue) 
            {
                minvalue = mag;
                minid = i;
            }
        }
        if (minid == -1) { return currentposition; }
        if ((navPoints[minid].transform.position - currentposition).sqrMagnitude < 2) {
            return navPoints[minid].navPoints[Random.Range(0, navPoints[minid].navPoints.Length)].transform.position; 
        }
        return navPoints[minid].transform.position;
    }
}
