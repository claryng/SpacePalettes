using UnityEngine;

[DefaultExecutionOrder(-3)]
public class ScreenUtils : MonoBehaviour
{
    public static float CameraHeight { get; private set; }
    public static float LeftPos { get; private set; }
    public static float RightPos { get; private set; }
    public static float DifferentDistToMoveObj { get; private set; } = 1.75f;

    [SerializeField] private float _padding = 1f;

    private void Start()
    {
        CameraHeight = Camera.main.orthographicSize + _padding;
        LeftPos = -DifferentDistToMoveObj;
        RightPos = DifferentDistToMoveObj;
    }
}
