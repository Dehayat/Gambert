using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsIcon : MonoBehaviour
{
    public GameObject Ps4;
    public GameObject Xbox;
    public GameObject Keyboard;


    private void OnEnable()
    {
        InputSystem.onDeviceChange += InputSystem_onDeviceChange;
    }


    private void OnDisable()
    {
        InputSystem.onDeviceChange -= InputSystem_onDeviceChange;
    }

    private void InputSystem_onDeviceChange(InputDevice device, InputDeviceChange change)
    {
        CheckGamePad(device);
    }

    private void CheckGamePad(InputDevice device)
    {

        Ps4.SetActive(false);
        Xbox.SetActive(false);
        Keyboard.SetActive(false);
        Debug.Log(device);
        if (device is Gamepad)
        {
            var dualShock = device as UnityEngine.InputSystem.DualShock.DualShockGamepad;
            if (dualShock != null)
            {
                Ps4.SetActive(true);
            }
            else
            {
                Xbox.SetActive(true);
            }
        }
        else
        {
            Keyboard.SetActive(true);
        }
    }
}
