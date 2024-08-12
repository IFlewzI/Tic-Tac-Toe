using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class SliderValueDisplay : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private TMP_Text _text;

    private void Start()
    {
        _text = GetComponent<TMP_Text>();
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
        _text.text = _slider.value.ToString();
    }

    private void OnSliderValueChanged(float newValue)
    {
        _text.text = newValue.ToString();
    }
}
