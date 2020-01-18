using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIComponentBase
{
    private GameObject m_componentObject;

    public UIComponentBase(GameObject componentObject)
    {
        this.m_componentObject = componentObject;

        UIRectTransform = this.m_componentObject.GetComponent<RectTransform>();

        InitUIComponent();
    }

    protected abstract void InitUIComponent();

    public virtual void SetActive(bool isActive)
    {
        m_componentObject.SetActive(isActive);
        if (isActive)
            OnUIComponentVisible();
        else
            OnUIComponentInvisible();
    }

    protected abstract void OnUIComponentVisible();

    protected abstract void OnUIComponentInvisible();


    public RectTransform UIRectTransform
    {
        get;
        private set;
    }

    public GameObject UIComponentObject => this.m_componentObject;

}
