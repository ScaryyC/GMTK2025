using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerPathTracker : MonoBehaviour
{
    public SplineContainer splineContainer;
    public GameObject trackingPlayer;
    Transform playerTransform;
    Spline currentPath;
    public float createKnotInterval = 3;

    void Start()
    {
        if(trackingPlayer == null)
        {
            Debug.Log("SET THE PLAYER TO BE TRACKED");
            return;
        }
        playerTransform = trackingPlayer.transform;
        StartSpline();
        StartCoroutine(CreateKnotCoroutine());
    }

    public void StartSpline()
    {
        if (splineContainer == null)
        {
            Debug.Log("No spline container attatched");
        }
        

        currentPath = splineContainer.AddSpline();
    }

    IEnumerator CreateKnotCoroutine()
    {
        yield return new WaitForSeconds(createKnotInterval);
        CreateKnot();
    }

    void CreateKnot()
    {
        if (currentPath == null)
        {
            Debug.Log("There is no current path");
            return;
        }
        float3 knotPosition = new float3(playerTransform.position.x, 0, playerTransform.position.z);
        currentPath.Add(knotPosition);
        StartCoroutine(CreateKnotCoroutine());
    }
}
