using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class CarManager : MonoBehaviour, IPointerDownHandler
{
    public static CarManager instance; // ����ʵ��;
    public List<SnapAble> snapAbleList;
    public List<FuelTank> fuelTankList;
    public List<PowerTank> powerTankList;

    public float totalFuelVolume;

    public float totalPowerVolume;

    public float totalFuel;

    public float totalPower;

    public TMP_Text totalPowerVolume_TMP;
    public TMP_Text totalPower_TMP;
    public TMP_Text totalFuelVolume_TMP;
    public TMP_Text totalFuel_TMP;
    //public TMP_Text totalBurnPowerRate_TMP;
    //public TMP_Text totalGainPowerRate_TMP;
    public TMP_Text warning;

    public float height;
    public float setCurrentPower;
    public float setPowerTankVolume;
    public float setPowerProductionRate;
    public float setClimbBurnPowerRate;
    public float totalBurnPowerRate;
    public float totalGainPowerRate;
    public bool isClimb;

    public GameObject Panel;
    public GameObject catPoint;
    public float techPoint;
    public Animator anim;

    public GameObject Dead;
    public void Start()
    {
        totalPowerVolume += setPowerTankVolume;
        totalGainPowerRate += setPowerProductionRate;
        anim = GetComponent<Animator>();
    }
    public void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }

    public void Update()
    {
        if (isClimb && totalPower == 0)
        {
            Dead.SetActive(true);         
        }
        BurnPower(totalBurnPowerRate * Time.deltaTime);//����
        if (fuelTankList != null)
        {
            totalFuel = 0; // ������ȼ��ֵ
            foreach (FuelTank fuelTank in fuelTankList)
            {
                totalFuel += fuelTank.currentFuel; // �ۼ�����ȼ�Ϲ޵ĵ�ǰȼ��ֵ
            }
        }
        totalPower = setCurrentPower; // ��ʼΪ���е�صĵ���
        if (powerTankList != null)
        {
            foreach (PowerTank powerTank in powerTankList)
            {
                totalPower += powerTank.currentPower; // �ۼ�������ӵ�صĵ���
            }
        }
        totalPowerVolume_TMP.text = "Total_Power_Volume: " + totalPowerVolume.ToString();
        totalPower_TMP.text = "Total_Power: " + totalPower.ToString("F0");
        totalFuelVolume_TMP.text = "Total_Fuel_Volume: " + totalFuelVolume.ToString();
        totalFuel_TMP.text = "Total_Fuel: " + totalFuel.ToString("F0");

        if (!AllBatteriesFull())
        {
            SetPowerSysterm();
        }
 
        TotalPowerBurnRate();
        //totalGainPowerRate_TMP.text = "TotalGainPowerRate: " + totalGainPowerRate.ToString();
        PowerWarning();
    }
    public void SetPowerSysterm()
    {
        float producedPower = setPowerProductionRate * Time.deltaTime;
        FillPower(producedPower);
    }
    public void FillFuel(float value)
    {
        if (fuelTankList != null)
        {
            float remainingFuel = value; // ʣ��ȼ����

            foreach (FuelTank fuelTank in fuelTankList)
            {
                if (!fuelTank.isFull)
                {
                    float availableSpace = fuelTank.volume - fuelTank.currentFuel;

                    if (remainingFuel <= availableSpace)
                    {
                        fuelTank.currentFuel += remainingFuel;
                        remainingFuel = 0;
                        break;
                    }
                    else
                    {
                        fuelTank.currentFuel += availableSpace;
                        remainingFuel -= availableSpace;
                    }
                }
            }

            if (remainingFuel > 0)
            {
                // ��ʾ: �������䶼�������޷�װ��ʣ���ȼ�ϡ�
                Debug.Log("All fuel tanks are full. Remaining fuel: " + remainingFuel);
            }
        }
    }
    public void BurnFuel(float value)
    {
        if (fuelTankList != null)
        {
            float remainingFuelToBurn = value; // ��Ҫ���ĵ�ȼ����

            foreach (FuelTank fuelTank in fuelTankList)
            {
                if (fuelTank.currentFuel > 0) // ��������ȼ�ϵ�����
                {
                    if (remainingFuelToBurn <= fuelTank.currentFuel)
                    {
                        fuelTank.currentFuel -= remainingFuelToBurn;
                        remainingFuelToBurn = 0;
                        break;
                    }
                    else
                    {
                        remainingFuelToBurn -= fuelTank.currentFuel;
                        fuelTank.currentFuel = 0;
                    }
                }
            }

            if (remainingFuelToBurn > 0)
            {
                // ��ʾ: ���������ȼ���������꣬����Ҫ����ȼ�ϡ�
                Debug.Log("Not enough fuel to burn. Remaining fuel needed: " + remainingFuelToBurn);
            }
        }
    }
    public void FillPower(float value)
    {
        if (powerTankList != null)
        {
            float remainingPower = value; // ʣ������

            // �ȸ����е�س��
            float availableSpaceInMainBattery = setPowerTankVolume - setCurrentPower;
            if (remainingPower <= availableSpaceInMainBattery)
            {
                setCurrentPower += remainingPower;
                remainingPower = 0;
            }
            else
            {
                setCurrentPower = setPowerTankVolume;
                remainingPower -= availableSpaceInMainBattery;
            }

            // Ȼ����б��еĵ�س��
            foreach (PowerTank powerTank in powerTankList)
            {
                if (!powerTank.isFull)
                {
                    float availableSpace = powerTank.volume - powerTank.currentPower;

                    if (remainingPower <= availableSpace)
                    {
                        powerTank.currentPower += remainingPower;
                        remainingPower = 0;
                        break;
                    }
                    else
                    {
                        powerTank.currentPower += availableSpace;
                        remainingPower -= availableSpace;
                    }
                }
            }

            if (remainingPower > 0)
            {
                // ��ʾ: ���е�ض��������޷�װ��ʣ���������
                Debug.Log("All power tanks are full. Remaining power: " + remainingPower);
            }
        }
    }
    public void BurnPower(float value)
    {
        if (powerTankList != null)
        {
            float remainingPowerToBurn = value; // ��Ҫ���ĵ�����

            // �������б��еĵ�ص���
            foreach (PowerTank powerTank in powerTankList)
            {
                if (powerTank.currentPower > 0) // �������������ĵ��
                {
                    if (remainingPowerToBurn <= powerTank.currentPower)
                    {
                        powerTank.currentPower -= remainingPowerToBurn;
                        remainingPowerToBurn = 0;
                        break;
                    }
                    else
                    {
                        remainingPowerToBurn -= powerTank.currentPower;
                        powerTank.currentPower = 0;
                    }
                }
            }

            // Ȼ�����Ĺ��е�ص���
            if (remainingPowerToBurn > 0 && setCurrentPower > 0)
            {
                if (remainingPowerToBurn <= setCurrentPower)
                {
                    setCurrentPower -= remainingPowerToBurn;
                    remainingPowerToBurn = 0;
                }
                else
                {
                    remainingPowerToBurn -= setCurrentPower;
                    setCurrentPower = 0;
                }
            }

            if (remainingPowerToBurn > 0)
            {
                // ��ʾ: ���е�ص������������꣬����Ҫ����������
                Debug.Log("Not enough power to burn. Remaining power needed: " + remainingPowerToBurn);
            }
        }
    }
    private bool AllBatteriesFull()
    {
        // �����е���Ƿ�����
        if (setCurrentPower < setPowerTankVolume)
        {
            return false; // ���е��δ��
        }

        // �������ӵ���б��������Ƿ�����
        if (powerTankList != null)
        {
            foreach (PowerTank powerTank in powerTankList)
            {
                if (!powerTank.isFull)
                {
                    return false; // ����δ������ӵ��
                }
            }
        }

        return true; // ���е�ؾ�������û����ӵ��
    }
    public void TotalPowerBurnRate()
    {
        if (!isClimb)
        {
            totalBurnPowerRate = 0f;
        }
        else
        {
            totalBurnPowerRate = setClimbBurnPowerRate ;
        }
        if (snapAbleList != null)
        {
            foreach (SnapAble snapAble in snapAbleList)
            {
                if (isClimb)
                {
                    totalBurnPowerRate += snapAble.powerLostRateMove;
                }
                else
                {
                    totalBurnPowerRate += snapAble.powerLostRateStill;
                }
            }
        }
        //totalBurnPowerRate_TMP.text = "TotalBurnPowerRate: " + totalBurnPowerRate.ToString();
    }
    public void dead()
    {
        SceneManager.LoadScene(1);
    }
    public void PowerWarning()
    {
        if (totalBurnPowerRate > totalGainPowerRate)
        {
            float netBurnRate = totalBurnPowerRate - totalGainPowerRate;
            if (netBurnRate > 0)
            {
                float timeToDepletion = totalPower / netBurnRate;
                warning.text = $"Warning: Power will deplete in {timeToDepletion:F2} seconds.";
            }
        }
        else if (totalBurnPowerRate < totalGainPowerRate)
        {
            warning.text = "Power is stable";
        }
        else
        {
            warning.text = "Power is balanced";
        }
    }
    public void StartClimb()
    {
        isClimb = true;
        Spawner.instance.isSpawning = true;
        Cat.instance.JumpUp(this.transform.position);
        anim.SetBool("run", true);
    }
    public void StopClimb()
    {
        isClimb = false;
        Spawner.instance.isSpawning = false;
        Cat.instance.JumpUp(catPoint.transform.position);
        anim.SetBool("run", false);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Panel.SetActive(true);
    }
}
