using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Model
{
    I,
    II,
    III,
}
public class Generator_Mech : SnapAble
{
    public float fuelConsumptionRate; // 燃料消耗速度（每秒消耗量）
    public float powerProductionRate; // 每秒产生的电力
    public Model model;
    public bool IsActive { get; private set; } = false; // 当前状态

    public GameObject controlPanel;

    public override void Start()
    {
        base.Start();
        AllocateValues();
    }
    public  override void Update()
    {
        base.Update();
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
        if (CarManager.instance.totalFuel> 0)
        {
            IsActive = true;
            
            //controlPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Not enough fuel to activate the generator!");
        }
    }

    public void ShutDown()
    {
        IsActive = false;
        //controlPanel.SetActive(false);
        CarManager.instance.totalGainPowerRate -= powerGainRate;
    }

    private void PerformTask()
    {
        CarManager.instance.BurnFuel(fuelConsumptionRate * Time.deltaTime);
        // 产生电力
        float producedPower = powerProductionRate * Time.deltaTime;
        CarManager.instance.FillPower(producedPower);
    }

    private void AllocateValues()
    {
        switch (model)
        {
            case Model.I:
                fuelConsumptionRate = 1f;
                powerProductionRate = 5f;
                break;
            case Model.II:
                fuelConsumptionRate = 3f;
                powerProductionRate = 20f;
                break;
            case Model.III:
                fuelConsumptionRate = 5f;
                powerProductionRate = 35f;
                break;
        }
    }
}