using System;
using UnityEngine;
using UnityEngine.UI;

public class SliderSettingComponent : UIComponentBase
{
    private Slider m_sliderComponent;
    private Text m_sliderValueText;

    private Action<SliderSettingComponent, float> m_onSliderValueChangedCallback;
    private Action<SliderSettingComponent, float> m_onDragEndCallback;

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
        this.m_onSliderValueChangedCallback?.Invoke(this, value);
        this.m_sliderValueText.text = value.ToString("0.00");
    }

    public void AddOnSliderValueChangedCallback(Action<SliderSettingComponent, float> callback)
    {
        m_onSliderValueChangedCallback += callback;
    }

    public void RemoveOnSliderValueChangedCallback(Action<SliderSettingComponent, float> callback)
    {
        m_onSliderValueChangedCallback -= callback;
    }

    public void AddOnSliderDragEndCallback(Action<SliderSettingComponent, float> callback)
    {
        m_onDragEndCallback += callback;
    }
    public void RemoveOnSliderDragEndCallback(Action<SliderSettingComponent, float> callback)
    {
        m_onDragEndCallback -= callback;
    }

    private void OnDragEndCallbackInternal(GameObject go, Vector2 delta)
    {
        m_onDragEndCallback?.Invoke(this, SliderComponent.value);
    }

    public void SetValue(float value)
    {
        this.m_sliderComponent.value = value;
        this.m_sliderValueText.text = value.ToString("0.00");
    }

    public void SetSliderMin(float minValue)
    {
        this.m_sliderComponent.minValue = minValue;

        SetValue(Mathf.Max(minValue, this.SliderComponent.value));
    }

    protected override void OnUIComponentInvisible()
    {
        this.m_sliderComponent.onValueChanged.RemoveListener(OnSliderValueChanged);
        SliderComponent.RemoveDragEndCallback(OnDragEndCallbackInternal);
    }

    public Slider SliderComponent => this.m_sliderComponent;

}
