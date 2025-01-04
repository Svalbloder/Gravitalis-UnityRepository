using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerTank : SnapAble
{
    public float volume;
    public float currentPower;
    public bool isFull;
    public Model model;
    public override void Start()
    {
        base.Start();
        AllocateValues();
        CarManager.instance.totalPowerVolume += volume;
        CarManager.instance.powerTankList.Add(this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (currentPower == volume)
        {
            isFull = true;
        }
        else
        {
            isFull = false;
        }
    }
    private void AllocateValues()
    {
        switch (model)
        {
            case Model.I:
                volume = 300f;
                break;
            case Model.II:
                volume = 500f;
                break;
            case Model.III:
                volume = 1000f; ;
                break;
        }
    }
}
