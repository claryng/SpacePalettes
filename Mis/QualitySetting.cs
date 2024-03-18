using UnityEngine;

[DefaultExecutionOrder(-4)]
public class QualitySetting : MonoBehaviour
{
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
}
