using GOGUI;
using UnityEngine.UI;
using static GOGUI.EventTriggerListener;

public static class ButtonExtension
{
    public static void AddClickCallback(this Button button, VoidDelegate callback)
    {
        EventTriggerListener.Get(button.gameObject).onClick -= callback;
        EventTriggerListener.Get(button.gameObject).onClick += callback;
    }
}
