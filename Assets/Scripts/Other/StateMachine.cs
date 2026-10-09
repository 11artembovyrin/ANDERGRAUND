using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IState
{
    void Enter();
    void Update();
    void FixedUpdate();
    void LateUpdate();
    void Exit();
}


public class StateMachine
{
    private IState currentState;

    public void SwitchState(IState newState)
    {
        if (currentState != null) 
            currentState.Exit();

        currentState = newState;

        if (currentState != null) 
            currentState.Enter();
    }

    public void Update()
    {
        if (currentState != null)
            currentState.Update();
    }

    public void FixedUpdate()
    {
        if (currentState != null)
            currentState.FixedUpdate();
    }

    public void LateUpdate()
    {
        if (currentState != null)
            currentState.LateUpdate();
    }

    public IState GetCurrentState()
    {
        return currentState;
    }
}
