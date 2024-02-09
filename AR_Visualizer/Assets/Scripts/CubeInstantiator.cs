using UnityEngine;

public class CubeInstantiator : MonoBehaviour
{
    public GameObject cubePrefab;
    public Transform cubeManager;

    public void InstantiateCube()
    {
        GameObject newCube = Instantiate(cubePrefab, cubeManager);
        // Position and scale the cube as needed
        newCube.transform.localPosition = Vector3.zero;
        newCube.transform.localScale = Vector3.one;
    }
}
