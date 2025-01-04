using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.GetComponent<CarManager>() != null)
        {
            CameraManager.instance.ChangeToNear();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.GetComponent<CarManager>() != null)
        {
            CameraManager.instance.ChangeBack();         
        }
    }
}
