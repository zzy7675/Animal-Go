using System.Collections;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    public static ObjectCreator instance;

    [Header("Traps")]
    public GameObject arrowPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    public void CreateObject(GameObject prefab, Transform target, float delay, bool shouldBeDestroyed = false)
    {
        StartCoroutine(CreateObjectRoutine(prefab, target, delay, shouldBeDestroyed));
    }
    private IEnumerator CreateObjectRoutine(GameObject prefab, Transform target, float delay, bool shouldBeDestroyed)
    {
        Vector3 newPosition = target.position;

        yield return new WaitForSeconds(delay);

        GameObject newObject = Instantiate(prefab, newPosition, Quaternion.identity);

        if (shouldBeDestroyed)
            Destroy(newObject, 15);
    }
}
