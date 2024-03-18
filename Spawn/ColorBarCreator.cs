using System.Collections.Generic;
using UnityEngine;
using ColorsAvailable;
using System;

public class ColorBarCreator : MonoBehaviour
{
    public Func<Colors, GameObject> OnGenerate { get; private set; }
    [Header("Bar Template")]
    [SerializeField] private GameObject _barTemplate;
    [SerializeField] private GameObject _purple;
    [SerializeField] private GameObject _green;
    [SerializeField] private GameObject _white;

    private const int _NumberOfColors = 3;

    private Transform _barTransform;
    private float[] _barTemplateChildrenPosX;

    private Colors[] _colorsToSpawn;
    private Dictionary<Colors, GameObject> _singleDict;

    private void Awake()
    {
        _barTransform = _barTemplate.transform;
        OnGenerate += GenerateColorBar;
    }

    private void OnDisable()
    {
        OnGenerate -= GenerateColorBar;
    }

    private void Start()
    {
        _colorsToSpawn = new Colors[_NumberOfColors];
        _singleDict = new Dictionary<Colors, GameObject>();
        _singleDict[Colors.Purple] = _purple;
        _singleDict[Colors.Green] = _green;
        _singleDict[Colors.White] = _white;

        _barTemplateChildrenPosX = new float[_NumberOfColors];   
        for(var i = 0; i < _NumberOfColors; i++)
        {
            _barTemplateChildrenPosX[i] = _barTransform.GetChild(i).position.x;
        }
    }

    /// <summary>
    /// Find a random position for colors in a color bar
    /// </summary>
    /// <param name="mustExist"></param>
    /// <returns></returns>
    private Colors[] RandomizeColorsToSpawn(Colors mustExist)
    {
        int posOfMustExist = UnityEngine.Random.Range(0, _colorsToSpawn.Length);
        _colorsToSpawn[posOfMustExist] = mustExist;

        // Randomize position for other colors
        for (var i = 0; i < _colorsToSpawn.Length; i++)
        {
            int randomColor = UnityEngine.Random.Range(0, _NumberOfColors + 1);
            if (i != posOfMustExist)
            {
                _colorsToSpawn[i] = (Colors)randomColor;
            }
        }

        return _colorsToSpawn;
    }

    /// <summary>
    /// Create a color bar game object based on the random positions
    /// </summary>
    /// <param name="mustExist"></param>
    /// <returns></returns>
    private GameObject GenerateColorBar(Colors mustExist)
    {
        _colorsToSpawn = RandomizeColorsToSpawn(mustExist);

        GameObject bar = new();
        bar.transform.position = Vector3.zero;
        for (var i = 0; i < _colorsToSpawn.Length; i++)
        {
            Colors color = _colorsToSpawn[i];

            if (!_singleDict.ContainsKey(color)) continue;

            GameObject newColor = Instantiate(_singleDict[color]);
            newColor.SetActive(true);

            Transform newColorTransform = newColor.transform;
            // Add colors as children to the color bar object
            newColorTransform.parent = bar.transform;

            // Place the colors in the position specified by the template
            Vector3 newPos = Helpers.GetVector3X(_barTemplateChildrenPosX[i]);
            newColorTransform.position = newPos;
        }

        // Add script to make the bar move
        bar.AddComponent<MoveScript>();
        return bar;
    }

}
