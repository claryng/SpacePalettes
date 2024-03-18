using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedObjectsController : MonoBehaviour
{
    public static event Action OnDissolve;
    public event Action<AudioClip> OnPlaySFX;
    public event Action<string, Vector3> OnDestroyHitBars;

    [SerializeField] private Spawner _spawner;
    [SerializeField] private Fading _fading;
    [SerializeField] private CountDown _countDown;
    [SerializeField] private Player _player;
    [SerializeField] private AudioClip _sfxDissolve;

    private List<GameObject> _spawnedObjects;

    private static SpawnedObjectsController instance;

    private void Awake()
    {
        ManageSingleton();
    }

    private void OnEnable()
    {
        _player.OnDestroySpawned += DestroySpawned;
        _player.OnDestroyHitObject += DestroyHitBar;
        _countDown.OnDestroyAfterWatchAd += DestroySpawned;
        _fading.OnFreezeMovement += FreezeMovement;
    }

    private void OnDisable()
    {
        _player.OnDestroySpawned -= DestroySpawned;
        _player.OnDestroyHitObject -= DestroyHitBar;
        _countDown.OnDestroyAfterWatchAd -= DestroySpawned;
        _fading.OnFreezeMovement -= FreezeMovement;
    }
    private void ManageSingleton()
    {
        if (instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void DestroySpawned()
    {
        if (_spawner)
        {
            _spawnedObjects = _spawner.SpawnedObjects; 
            if (_spawnedObjects.Count > 0)
            {
                OnPlaySFX?.Invoke(_sfxDissolve);
                for(var i = 0; i < _spawnedObjects.Count; i++)
                {
                    GameObject objectToBeDeleted = _spawnedObjects[i];

                    if(objectToBeDeleted != null)
                    {
                        foreach (Transform child in objectToBeDeleted.transform)
                        {
                            if (child != null)
                            {
                                Dissolve dissolve = child.GetComponent<Dissolve>();

                                if (dissolve != null)
                                {
                                    OnDissolve?.Invoke();
                                }
                            }
                        }
                    }
                }
            }
            _spawnedObjects.Clear();
        }
    }

    private void FreezeMovement()
    {
        if(_spawner && _spawner.gameObject.activeSelf)
        {
            _spawnedObjects = _spawner.SpawnedObjects;

            if(_spawnedObjects.Count > 0)
            {
                for(var i = 0; i < _spawnedObjects.Count; i++)
                {
                    GameObject obj = _spawnedObjects[i];

                    if (obj)
                    {
                        MoveScript scriptMove = obj.GetComponent<MoveScript>();

                        if(scriptMove != null && scriptMove.gameObject.activeSelf)
                        {
                            scriptMove.gameObject.SetActive(false); 
                        }
                    }
                }
            }
            _spawner.gameObject.SetActive(false);
        }
    }

    private void DestroyHitBar(GameObject hitObject)
    {
        #region Reduce alpha
        Transform t = hitObject.transform;
        if (t.parent != null)
        {
            Transform p = t.parent.transform;
            foreach (Transform child in p)
            {
                if (child)
                {

                    if(child.TryGetComponent<BoxCollider2D>(out var boxCollider2D))
                    {
                        Destroy(boxCollider2D);
                    }
                    
                    if (child.TryGetComponent<Dissolve>(out var dissolve))
                    {
                        Destroy(dissolve);
                    }

                    OnDestroyHitBars?.Invoke(child.tag, child.position);
                    child.gameObject.SetActive(false);
                }
            }
        }
        #endregion 

    }

}
