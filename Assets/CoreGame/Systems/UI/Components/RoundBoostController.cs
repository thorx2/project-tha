using System;
using Lean.Gui;
using ProjTha;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.Audio;

public class RoundBoostController : MonoBehaviour
{
    [SerializeField]
    private LeanWindow leanWindow;


    [SerializeField]
    private AudioMixerSnapshot onShow;

    [SerializeField]
    private AudioMixerSnapshot onHide;

    protected void Start()
    {
        leanWindow.OnOff.AddListener(OnUIClosed);
    }

    private void OnUIClosed()
    {
        onHide.TransitionTo(0.2f);
        GameManager.PauseElementMovement = false;
        Physics.simulationMode = SimulationMode.FixedUpdate;
    }

    public void ShowUI()
    {
        onShow.TransitionTo(0.2f);
        GameManager.PauseElementMovement = true;
        leanWindow.On = true;
        Physics.simulationMode = SimulationMode.Script;
    }

    private void OnButtonSelected()
    {
        leanWindow.On = false;
    }

    public void IncreasedAttackDamageSelect()
    {
        GameManager.Instance.BoostSelected(EBoostType.EDamage);
        OnButtonSelected();
    }

    public void IncreasedHPSelect()
    {
        GameManager.Instance.BoostSelected(EBoostType.EHealth);
        OnButtonSelected();
    }

    public void IncreasedFireRateSelect()
    {
        GameManager.Instance.BoostSelected(EBoostType.ERateOfFire);
        OnButtonSelected();
    }
}
