using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuUI : Singleton<PauseMenuUI>
{
    public void OnPauseMenu()
    {
        GameManager.Instance.GamePaused = true;
    }
}
