using System.Collections.Generic;
using UnityEngine;
public static class Helpers
{
    public static string BestScoreKey { get; private set; } = "SpacePalettes_BestScore";
    public static string TutorialKey { get; private set; } = "SpacePalettes_TutorialFirstTimePlaying";
    private static readonly Dictionary<float, WaitForSeconds> s_waitForSecondsDict = new();
    private static readonly Dictionary<(float, float), Vector3> s_vector3Y = new();
    private static readonly Dictionary<float, Vector3> s_vector3X = new();
    private static readonly WaitForEndOfFrame s_waitForEndOfFrame = new();

    /// <summary>
    /// Instead of using "new WaitForSeconds(seconds)", just grab an already existing WaitForSeconds in the Dict.
    /// </summary>
    /// <param name="waitSecond"></param>
    /// <returns></returns>
    public static WaitForSeconds GetWaitForSeconds(float waitSecond)
    {
        if (s_waitForSecondsDict.TryGetValue(waitSecond, out WaitForSeconds waitSeconds)) { return waitSeconds; }

        s_waitForSecondsDict[waitSecond] = new WaitForSeconds(waitSecond);

        return s_waitForSecondsDict[waitSecond];
    }

    public static Vector3 GetVector3Y(float y, float x)
    {
        if(s_vector3Y.TryGetValue((x,y), out Vector3 vect)) { return vect; }

        s_vector3Y[(x,y)] = new Vector3(x,y,0f);
        return s_vector3Y[(x,y)];
    }

    public static Vector3 GetVector3X(float x)
    {
        if (s_vector3X.TryGetValue(x, out Vector3 vect)) { return vect; }

        s_vector3X[x] = new Vector3(x, ScreenUtils.CameraHeight, 0f);
        return s_vector3X[x];
    }

    public static WaitForEndOfFrame GetWaitForEndOfFrame()
    {
        return s_waitForEndOfFrame;
    }

}