using UnityEngine;
using SimpleJSON;
using System.IO;
using TMPro;

public class TimeCycle : MonoBehaviour
{
    [Range(0.00001f, 1)]
    public float timeScale = 1;
    [Range(0, 360)]
    public float lightFactor = 360;

    [Space][Header("Time Trackers")]
    public float hourCount = 0;
    public int dayCount = 0;
    public int monthCount = 0;
    public int yearCount = 0;

    [Space][Header("Sky Modifications")]
    public Light directionalLight;
    public Gradient ambientColor;
    public Gradient directionalLightColor;
    public Gradient fogColor;
    public Gradient skyTintColor;
    public AnimationCurve exposureCurve;
    public AnimationCurve fogDensityCurve;

    //Thresholds
    public const int HoursPerDay = 24;
    public const int DaysPerMonth = 30;
    public const int MonthsPerYear = 12;

    [Space] [Header("UI Contents")]
    public TextMeshProUGUI dateText;


    public delegate void YearEndEvent();
    public event YearEndEvent YearEndEvents;
   
    private void OnEnable()
    {
        YearEndEvents += OnYearEnd;
    }
    private void OnDisable()
    {
        YearEndEvents -= OnYearEnd;
    }

    private void Start()
    {
        UpdateCalendar();    
    }

    private void Update()
    {
        UpdateHour();
        UpdateSky(Delta());
    }

    private void UpdateSky(float delta)
    {
        RenderSettings.ambientLight = ambientColor.Evaluate(delta);
        RenderSettings.fogColor = fogColor.Evaluate(delta);
        RenderSettings.fogDensity = fogDensityCurve.Evaluate(delta);
        RenderSettings.skybox.SetFloat("_Exposure", exposureCurve.Evaluate(delta));
        RenderSettings.skybox.SetColor("_SkyTint", skyTintColor.Evaluate(delta));
        directionalLight.color = directionalLightColor.Evaluate(delta);
        UpdateLightRotation(delta);
    }

    private void UpdateLightRotation(float delta)
    {
        float x = (delta * lightFactor) - 90;
        directionalLight.transform.localRotation = Quaternion.Euler(x, 170, 15);
    }

    public float Delta()
    {
        return hourCount / HoursPerDay;
    }

    private void UpdateHour()
    {
        hourCount += timeScale * Time.deltaTime;
        if (hourCount > HoursPerDay)
        {
            hourCount = Mathf.Epsilon;
            UpdateDay();
        }
    }

    private void UpdateDay()
    {
        dayCount++;
        if (dayCount > DaysPerMonth)
        {
            dayCount = 1;
            UpdateMonth();
        }
        UpdateCalendar();
    }

    private void UpdateMonth()
    {
        monthCount++;
        if (monthCount > MonthsPerYear)
        {
            monthCount = 1;
            UpdateYear();
        }
        UpdateCalendar();
    }

    private void UpdateYear()
    {
        yearCount++;
        UpdateCalendar();
        TriggerYearEndEvent();
    }

    public float CurrentTime()
    {
        return hourCount;
    }

    public string CurrentDate()
    {
        string date = monthCount.ToString() + " / " + dayCount.ToString() + " / " + yearCount.ToString();
        return date;
    }

    private void UpdateCalendar()
    {
        dateText.text = CurrentDate();
    }

    private void TriggerYearEndEvent()
    {
        YearEndEvents?.Invoke();
    }

    private void OnYearEnd()
    {
        print("Year " + yearCount + " has ended. Congrats on surviving");
    }

    public void SaveData()
    {
        JSONObject json = new JSONObject();
        json.Add("hourCount", hourCount);
        json.Add("dayCount", dayCount);
        json.Add("monthCount", monthCount);
        json.Add("yearCount", yearCount);
        File.WriteAllText(DataPath(), json.ToString());
    }
    public void LoadDate()
    {
        JSONObject json = (JSONObject)JSON.Parse(File.ReadAllText(DataPath()));
        hourCount = json["hourCount"];
        dayCount = json["dayCount"];
        monthCount = json["monthCount"];
        yearCount = json["yearCount"];
        UpdateCalendar();
    }
    public void ClearData()
    {
        if (File.Exists(DataPath()))
            File.Delete(DataPath());
    }
    private string DataPath()
    {
        return Application.persistentDataPath + "/ingametimecycinfo.dat";
    }
}
