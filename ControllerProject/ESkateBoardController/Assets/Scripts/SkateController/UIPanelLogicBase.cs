using UnityEngine;
using System.Collections;

public class UIPanelLogicBase : IUIPanelLogic
{
    protected RectTransform m_panelRootObject = null;
    public string PanelName = string.Empty;

    public UIPanelLogicBase(RectTransform uiPanelRootTransfrom)
    {
        this.m_panelRootObject = uiPanelRootTransfrom;
    }

    public virtual void OnCreate()
    {
    }

    public virtual void OnEnter(params object[] onEnterParams)
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void OnUpdate()
    {
    }
}
