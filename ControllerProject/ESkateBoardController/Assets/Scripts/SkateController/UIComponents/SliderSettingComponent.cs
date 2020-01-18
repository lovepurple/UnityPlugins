using GOGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSettingComponent : UIComponentBase
{
    private Slider m_sliderComponent;
    private Text m_sliderValueText;

    private Action<float> m_onSliderValueChangedCallback;
    private Action<float> m_onDragEndCallback;

    public SliderSettingComponent(GameObject gameObject) : base(gameObject) { }

    protected override void InitUIComponent()
    {
        this.m_sliderComponent = UIRectTransform.Find("Slider").GetComponent<Slider>();
        this.m_sliderValueText = UIRectTransform.Find("Text").GetComponent<Text>();
    }

    protected override void OnUIComponentVisible()
    {
        this.m_sliderComponent.onValueChanged.AddListener(OnSliderValueChanged);
        SliderComponent.AddOnDragEndCallback(OnDragEndCallbackInternal);
    }

    private void OnSliderValueChanged(float value)
    {
        this.m_onSliderValueChangedCallback?.Invoke(value);
        this.m_sliderValueText.text = String.Format("{0:F}", value);
    }

    public void AddOnSliderValueChangedCallback(Action<float> callback)
    {
        m_onSliderValueChangedCallback += callback;
    }

    public void RemoveOnSliderValueChangedCallback(Action<float> callback)
    {
        m_onSliderValueChangedCallback -= callback;
    }

    public void AddOnSliderDragEndCallback(Action<float> callback)
    {
        m_onDragEndCallback += callback;
    }
    public void RemoveOnSliderDragEndCallback(Action<float> callback)
    {
        m_onDragEndCallback -= callback;
    }

    private void OnDragEndCallbackInternal(GameObject go, Vector2 delta)
    {
        m_onDragEndCallback?.Invoke(SliderComponent.value);
    }




    public void SetValue(float value)
    {
        this.m_sliderComponent.value = value;
        this.m_sliderValueText.text = value.ToString();
    }

    protected override void OnUIComponentInvisible()
    {
        this.m_sliderComponent.onValueChanged.RemoveListener(OnSliderValueChanged);
        SliderComponent.RemoveDragEndCallback(OnDragEndCallbackInternal);
    }

    public Slider SliderComponent => this.m_sliderComponent;
}
