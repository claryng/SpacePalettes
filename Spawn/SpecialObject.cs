using System;
using UnityEngine;

public class SpecialObject : MonoBehaviour
{
    public Func<GameObject, GameObject> OnGenerateSpecialObject { get; private set; }

    private void OnEnable()
    {
        OnGenerateSpecialObject += GenerateObject;
    }

    private void OnDisable()
    {
        OnGenerateSpecialObject -= GenerateObject;
    }

    /// <summary>
    /// Randomize the position of the speical object
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomSpecialPos()
    {
        int rnd = UnityEngine.Random.Range(0, 3);

        if (rnd == 0)
        {
            return Helpers.GetVector3X(ScreenUtils.LeftPos);
        }
        else if (rnd == 1)
        {
            return Helpers.GetVector3X(ScreenUtils.RightPos);
        }

        return Helpers.GetVector3X(0f);
    }

    private GameObject GenerateObject(GameObject objectToGenerate)
    {
        GameObject obj =  Instantiate(objectToGenerate, GetRandomSpecialPos(), Quaternion.identity);
        obj.SetActive(true);
        return obj;
    }
}
