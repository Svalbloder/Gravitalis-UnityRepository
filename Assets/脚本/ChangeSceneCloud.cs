using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneCloud : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision!=null && collision.GetComponent<CarManager>()!= null)
        {
            BackgroundManager.instance.SwitchBackgroundList();
            Destroy(this);
        }
    }
}
