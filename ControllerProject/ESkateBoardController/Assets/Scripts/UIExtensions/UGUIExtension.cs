using GOGUI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GOGUI.EventTriggerListener;

public static class UGUIExtension
{
    public static void AddClickCallback(this Button button, VoidDelegate callback)
    {
        EventTriggerListener.Get(button.gameObject).onClick -= callback;
        EventTriggerListener.Get(button.gameObject).onClick += callback;
    }

    public static void AddClickCallback(this Image imageWithRaycast, VoidDelegate callback)
    {
        EventTriggerListener.Get(imageWithRaycast.gameObject).onClick -= callback;
        EventTriggerListener.Get(imageWithRaycast.gameObject).onClick += callback;
    }

    public static void AddClickCallback(this MaskableGraphic graphicWithRayCast, VoidDelegate callback)
    {
        EventTriggerListener.Get(graphicWithRayCast.gameObject).onClick -= callback;
        EventTriggerListener.Get(graphicWithRayCast.gameObject).onClick += callback;
    }

    public static void RemoveClickCallback(this MaskableGraphic graphicWithRayCast, VoidDelegate callback)
    {
        EventTriggerListener.Get(graphicWithRayCast.gameObject).onClick -= callback;
    }

    public static void SetActive(this Graphic uiGraphic, bool isActive)
    {
        uiGraphic.gameObject.SetActive(isActive);
    }
}
