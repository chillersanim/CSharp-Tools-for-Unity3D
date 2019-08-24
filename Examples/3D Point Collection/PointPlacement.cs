using System.Collections;
using System.Collections.Generic;
using Unity_Tools.Core;
using UnityEngine;

public class PointPlacement : MonoBehaviour
{
    public Vector3 min = Vector3.one * -10;

    public Vector3 max = Vector3.one * 10;

    public int Amount = 1000;

    public GameObject Prefab;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (Prefab == null)
        {
            return;
        }

        for (var i = 0; i < Amount; i++)
        {
            var position = new Vector3(Random.value, Random.value, Random.value);
            position = min + position.ScaleComponents((max - min));

            Instantiate(Prefab, position, Random.rotation, this.transform);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(min + (max - min) / 2f, max - min);
    }
}
