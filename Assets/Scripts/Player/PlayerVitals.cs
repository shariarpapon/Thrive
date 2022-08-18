using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;

[System.Serializable]
public class VitalData
{
    public float currentHealth;
    public float currentEnergy;
    [Space]
    public float maxHealth;
    public float maxEnergy;
    [Space]
    public float autoHealthRegenRate;
    public float autoEnergyBurnRate;
    public float healthLossRateDueToLowEnergy;
    public float energyBurnDueToLowHealthPercentage;
    public float energyBurnRateMovingPercentage;
    internal float autoEnergyBurnDueToLowHealth;
}

public class PlayerVitals : MonoBehaviour
{
    public static bool IsAlive = true;
    public VitalData vital;

    private void Start()
    {
        if (File.Exists(DataPath()) == false)
            InitializeVitalSystem();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            InflictDamage(10);
        }

        VitalThresholdEffects();

        if (Alive() == false)
        {
            IsAlive = false;
            print("Player Died");
        }
    }

    private void InitializeVitalSystem()
    {
        vital.currentHealth = vital.maxHealth;
        vital.currentEnergy = vital.maxEnergy;
        vital.autoEnergyBurnDueToLowHealth = vital.autoEnergyBurnRate + (vital.autoEnergyBurnRate * vital.energyBurnDueToLowHealthPercentage);
    }

    private void VitalThresholdEffects()
    {
        if (vital.currentEnergy > vital.maxEnergy)
        {
            vital.currentEnergy = vital.maxEnergy;
        }
        else if (vital.currentEnergy > 0)
        {
            if (vital.currentHealth == vital.maxHealth)
                vital.currentEnergy -= EnergyBurnRate * Time.deltaTime;
        }
        else if (vital.currentEnergy < 0)
        {
            vital.currentEnergy = 0;
        }
        else if (vital.currentEnergy == 0)
        {
            vital.currentHealth -= vital.healthLossRateDueToLowEnergy * Time.deltaTime;
        }

        if (vital.currentHealth <= 0)
        {
            vital.currentHealth = 0;
            IsAlive = false;
        }
        else if (vital.currentHealth > vital.maxHealth)
            vital.currentHealth = vital.maxHealth;

        else if (vital.currentHealth < vital.maxHealth && vital.currentHealth > 0 && vital.currentEnergy > 0)
        {
            vital.currentHealth += vital.autoHealthRegenRate * Time.deltaTime;
        }
    }

    public float EnergyBurnBuff()
    {
        float rate = vital.autoEnergyBurnRate + (vital.autoEnergyBurnRate * vital.energyBurnDueToLowHealthPercentage);
        return rate;
    }

    public float EnergyBurnRate
    {
        get
        {
            float returnValue = vital.autoEnergyBurnRate;
            if (FPController.Moving())
            {
                returnValue = (returnValue * vital.energyBurnRateMovingPercentage) + returnValue;
            }
            if (vital.currentHealth < vital.maxHealth)
            {
                returnValue = returnValue + (returnValue * vital.energyBurnDueToLowHealthPercentage);
            }
            return returnValue;
        }
        internal set { vital.autoEnergyBurnRate = value; }
    }

    public bool InflictDamage(float amount)
    {
        vital.currentHealth -= amount;

        return Alive();
    }

    public void AddHealth(float amount)
    {
        vital.currentHealth += amount;
    }

    public void AddEnergy(float amount)
    {
        vital.currentEnergy += amount;
    }

    public bool Alive()
    {
        if (vital.currentHealth <= 0)
            return false;

        return true;
    }

    public void SaveData()
    {
        JSONObject js = new JSONObject();
        js.Add("lifeState", Alive());
        js.Add("currentHealth", vital.currentHealth);
        js.Add("currentEnergy", vital.currentEnergy);
        js.Add("maxHealth", vital.maxHealth);
        js.Add("maxEnergy", vital.maxEnergy);
        js.Add("autoEnergyBurnRate", vital.autoEnergyBurnRate);
        js.Add("autoEnergyBurnRateDueToLowHealth", vital.autoEnergyBurnDueToLowHealth);
        js.Add("autoHealthRegenRate", vital.autoHealthRegenRate);
        js.Add("healthLossDueToLowEnergy", vital.healthLossRateDueToLowEnergy);
        js.Add("energyBurnRateMovingPercentage", vital.energyBurnRateMovingPercentage);
        js.Add("energyBurnDueToLowHealthPercentage", vital.energyBurnDueToLowHealthPercentage);
        File.WriteAllText(DataPath(), js.ToString());
    }
    public void LoadData()
    {
        if (!File.Exists(DataPath()))
            return;

        JSONObject js = (JSONObject)JSON.Parse(File.ReadAllText(DataPath()));
        IsAlive = js["lifeState"];
        vital.currentHealth = js["currentHealth"];
        vital.currentEnergy = js["currentEnergy"];
        vital.maxHealth = js["maxHealth"];
        vital.maxEnergy = js["maxEnergy"];
        vital.autoEnergyBurnRate = js["autoEnergyBurnRate"];
        vital.autoEnergyBurnDueToLowHealth = js["autoEnergyBurnDueToLowHealth"];
        vital.autoHealthRegenRate = js["autoHealthRegenRate"];
        vital.healthLossRateDueToLowEnergy = js["healthLossDueToLowEnergy"];
        vital.energyBurnRateMovingPercentage = js["energyBurnRateMovingPercentage"];
        vital.energyBurnDueToLowHealthPercentage = js["energyBurnDueToLowHealthPercentage"];
    }
    public void ClearData()
    {
        if (File.Exists(DataPath()))
        {
            File.Delete(DataPath());
        }
    }

    private string DataPath()
    {
        return Application.persistentDataPath + "/playervitalinfo.dat";
    }
}
