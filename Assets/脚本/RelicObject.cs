using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicObject : SnapAble
{
    public float techPoint;

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (stickObj != null)
        {
            if (stickObj.GetComponent<Lab>() != null)
            {
                stickObj.GetComponent<Lab>().techPoints = techPoint;
                stickObj.GetComponent<Lab>().currentRelic.SetActive(true);
                Destroy(this.gameObject);
                Debug.Log("科技点以传送");
            }
        }  
    }
}
