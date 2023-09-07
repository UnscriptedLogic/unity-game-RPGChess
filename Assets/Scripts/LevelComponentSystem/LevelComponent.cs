using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelComponent : MonoBehaviour
{
    protected LevelController level;

    protected virtual void Start()
    {
        level = LevelController.instance;

        level.OnLevelInitialized += Level_OnLevelInitialized;
    }

    private void Level_OnLevelInitialized(object sender, System.EventArgs e)
    {
        OnControllerInitialized();
    }

    protected abstract void OnControllerInitialized();
}
