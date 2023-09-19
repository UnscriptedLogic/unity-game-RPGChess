using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHandler : MonoBehaviour
{
    private void Start()
    {
        ToggleCursorVisible(false);
    }

    public void ToggleCursorVisible(bool value)
    {
        if (value)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        Cursor.visible = value;
    }
}
