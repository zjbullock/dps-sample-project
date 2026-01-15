using DPS.SimpleUIFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISampleMainPageController : MenuControllerTemplate
{
    [SerializeField]
    private MenuButtonController _button;

    [SerializeField]
    private int count = 0;

    protected override void ResetMenu()
    {
        base.ResetMenu();
        _button.OnClick.RemoveAllListeners();
        base.onSubmitInputEvent.RemoveAllListeners();
    }

    protected override void SetCurrentMenu()
    {
        this.ResetMenu();
        this.SetButtonText();
        _button.OnClick.AddListener(Increment);
        base.onSubmitInputEvent.AddListener(Increment);
    }

    private void Increment()
    {
        Debug.Log("Increment Fired");
        if (this._button == null)
        {
            return;
        }
        count++;
        this.SetButtonText();
    }

    private void SetButtonText()
    {
        this._button.SetButtonContent(null, count + "");
    }
}