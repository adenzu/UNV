using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderText : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _text;
    [SerializeField] private Format _format;

    private string _defaultText;

    private void Awake()
    {
        _defaultText = _text.text;
    }

    public void SetText(float value)
    {
        _text.text = _defaultText + ": " + value.ToString(_format == Format.Whole ? "0" : "0.00");
    }

    private enum Format
    {
        Whole,
        Float,
    }
}
