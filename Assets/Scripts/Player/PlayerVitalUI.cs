using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerVitalUI : MonoBehaviour
{
    internal const float FillValueScale = 100;

    public PlayerVitals playerVital;

    public Image healthBar;
    public Image energyBar;

    public TextMeshProUGUI healthTMP;
    public TextMeshProUGUI energyTMP;

    private void Update()
    {
        UpdateVitals();
    }

    public void UpdateVitals()
    {
        energyBar.fillAmount = playerVital.vital.currentEnergy / FillValueScale;
        energyTMP.text = Mathf.RoundToInt(playerVital.vital.currentEnergy).ToString() + " %";

        healthBar.fillAmount = playerVital.vital.currentHealth / FillValueScale;
        healthTMP.text = Mathf.RoundToInt(playerVital.vital.currentHealth).ToString() + " %";

    }
}
