using System.Collections;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    private Transform _t;
    private float _x;

    // Start is called before the first frame update
    private void Start()
    {
        _t = transform;
        _x = _t.position.x;
        StartCoroutine(MoveCoroutine());
    }
    private IEnumerator MoveCoroutine()
    {
        for(var t = _t.position.y; t >= -ScreenUtils.CameraHeight * 2; 
            t -= Time.deltaTime * InteractablesMovementUtils._moveSpeed)
        {
            _t.position = Helpers.GetVector3Y(t, _x);
            InteractablesMovementUtils.OnIncrementSpeed?.Invoke();
            yield return Helpers.GetWaitForEndOfFrame();
        }

        gameObject.SetActive(false);
        Destroy(gameObject); 
    }

}
