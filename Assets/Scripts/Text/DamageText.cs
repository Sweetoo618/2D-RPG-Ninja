using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private TextMeshProUGUI damageTMP;

    public void SetDamageText(float damage)
    {
        SetDamageText(damage, false);
    }

    public void SetDamageText(float damage, bool isCritical)
    {
        if (damageTMP == null) return;
        damageTMP.text = isCritical ? $"CRIT! {damage:0}" : $"{damage:0}";
        damageTMP.color = isCritical ? new Color(1f, 0.75f, 0.08f) : Color.white;
        damageTMP.fontSize = isCritical ? 32f : 24f;

    }

    public void DestroyText()
    {
        Destroy(gameObject);
    }
}
