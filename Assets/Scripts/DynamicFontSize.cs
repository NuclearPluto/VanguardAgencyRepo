using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicFontSize : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private float newFontSize;

    void start() {
        SetFontSize(newFontSize);
    }

    public void SetFontSize(float fontSize)
    {
        textMeshPro.fontSize = fontSize;
    }

    public void ChangeFontSize()
    {
        SetFontSize(newFontSize);
    }
}
