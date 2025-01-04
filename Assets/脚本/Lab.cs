using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab : SnapAble
{
    public float techPointProductionRate;
    public bool IsActive { get; private set; } = false; // 当前状态

    public GameObject controlPanel;
    public GameObject currentRelic;
    public float techPoints;
    void Update()
    {
        if (IsActive)
        {
            if (CarManager.instance.totalFuel > 0)
            {
                PerformTask();
            }
            else
            {
                Debug.LogWarning("Fuel depleted! Shutting down the generator.");
                ShutDown();
            }
        }
    }
    //public override void ToActivate()
    //{
    //    controlPanel.SetActive(true);
    //    CarManager.instance.totalGainPowerRate += powerGainRate;
    //}

    public void Activate()
    {
        if (CarManager.instance.totalFuel > 0)
        {
            IsActive = true;

            controlPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Not enough fuel to activate the generator!");
        }
    }

    public void ShutDown()
    {
        IsActive = false;
        controlPanel.SetActive(false);
        CarManager.instance.totalGainPowerRate -= powerGainRate;
    }

    private void PerformTask()
    {
        if (techPoints > 0)
        {
            CarManager.instance.techPoint += techPointProductionRate * Time.deltaTime;
            techPoints -= techPointProductionRate * Time.deltaTime;
        }
        else
        {
            Debug.Log("吸收完成");
            currentRelic.SetActive(false);
        }
       
    }

}
