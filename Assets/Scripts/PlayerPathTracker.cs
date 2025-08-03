using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PlayerPathTracker : MonoBehaviour
{
    public SplineContainer splineContainer;
    public GameObject trackingPlayer;
    Transform playerTransform;
    Spline currentPath;
    public float createKnotInterval = 3;
    public float splineHeight = 100f;
    public bool matchPlayerHeight = false;
    bool pathCompleted = false;

    [Header("Loop Quality Check")]
    public float distanceGood = 10f;
    public float distanceOkay = 20f;
    public float distanceBad = 50f;

    [Header("Trace Path")]
    SplineExtrude splineExtrude;
    MeshRenderer extrudeRenderer;
    public float traceSpeed = 1f;
    public float traceInterval = 0.2f;

    [Header("DEBUG")]
    public bool showDistances;
    public bool ignoreTowers;

    void Start()
    {
        if (trackingPlayer == null)
        {
            Debug.Log("SET THE PLAYER TO BE TRACKED");
            return;
        }
        playerTransform = trackingPlayer.transform;
        splineExtrude = GetComponent<SplineExtrude>();
        extrudeRenderer = GetComponent<MeshRenderer>();
        if (splineExtrude != null)
        {
            extrudeRenderer.enabled = false;
        }
        StartSpline();
    }

    private void OnEnable()
    {
        GameManager.onStartPathTracing += TraceSplinePath;
    }

    private void OnDisable()
    {
        GameManager.onStartPathTracing -= TraceSplinePath;
    }

    public void StartSpline()
    {
        if (splineContainer == null)
        {
            Debug.Log("No spline container attatched");
        }
        
        currentPath = splineContainer.AddSpline();
        CreateKnot();
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

        if (pathCompleted)
        {
            return;
        }


        Vector3 playerPos = playerTransform.position;
        if (matchPlayerHeight)
        {
            splineHeight = playerPos.y;
        }

        float3 knotPosition = new float3(playerPos.x, splineHeight, playerPos.z);
        currentPath.Add(knotPosition);
        StartCoroutine(CreateKnotCoroutine());
    }

    public void CompletePath()
    {
        if (currentPath == null)
        {
            Debug.Log("There is no current path");
            return;
        }

        if ((AreTowersVisisted() || ignoreTowers) && !pathCompleted)
        {
            CreateKnot();
            StopCoroutine(CreateKnotCoroutine());
            CalculateLoopQuality();
            GameManager.onPathCompleted?.Invoke();
            pathCompleted = true;
        }
    }

    void CalculateLoopQuality()
    {
        GameManager gm = GameObject.FindAnyObjectByType<GameManager>();
        if (gm == null)
        {
            Debug.Log("GameManager not found");
            return;
        }
        BezierKnot firstKnot = currentPath.Knots.ElementAt(0);
        BezierKnot lastKnot = currentPath.Knots.Last();
        Vector2 firstKnotPosition = new Vector2(firstKnot.Position.x, firstKnot.Position.z);
        Vector2 lastKnotPosition = new Vector2(lastKnot.Position.x, lastKnot.Position.z);
        float closingLoopDistance = Vector2.Distance(lastKnotPosition, firstKnotPosition);
        Debug.Log("Distance: " + closingLoopDistance);
        if (closingLoopDistance >= distanceBad)
        {
            gm.SetPlayerLoopQuality(GameManager.LoopQuality.Bad);
            Debug.Log("Your loop is: BAD");
        }
        else if (closingLoopDistance >= distanceOkay)
        {
            gm.SetPlayerLoopQuality(GameManager.LoopQuality.Okay);
            Debug.Log("Your loop is: OKAY");
        }
        else if (closingLoopDistance >= distanceGood)
        {
            gm.SetPlayerLoopQuality(GameManager.LoopQuality.Good);
            Debug.Log("Your loop is: GOOD");
        }
        else
        {
            gm.SetPlayerLoopQuality(GameManager.LoopQuality.Amazing);
            Debug.Log("Your loop is: AMAZING");
        }

        if (showDistances)
        {
            Vector3 knot1 = new Vector3(firstKnotPosition.x, 0, firstKnotPosition.y);
            Vector3 knot2 = new Vector3(lastKnotPosition.x, 0, lastKnotPosition.y);
            Debug.DrawRay(knot1, Vector3.up, Color.green, 1000);
            Debug.DrawRay(knot2, Vector3.up, Color.green, 1000);
            Debug.DrawLine(knot1, knot2, Color.red, 1000);
        }
    }

    bool AreTowersVisisted()
    {
        GameManager gm = GameObject.FindAnyObjectByType<GameManager>();
        if (gm == null)
        {
            Debug.Log("GameManager not found");
            return false;
        }
        if (GameManager.GetTowersArrayLength() == 0)
        {
            Debug.Log("Towers array is empty");
            return true;
        }

        bool passed = true;

        foreach (GameObject tower in gm.towersArray)
        {
            Tower towerScript = tower.GetComponent<Tower>();
            if (towerScript != null)
            {
                if (!towerScript.visisted)
                {
                    passed = false;
                }
            }
        }
        return passed;
    }
    void TraceSplinePath()
    {
        if (extrudeRenderer == null)
        {
            Debug.Log("Renderer is null");
            return;
        }

        extrudeRenderer.enabled = true;
        StartCoroutine(ExtrudeSplineCoroutine());
    }

    IEnumerator ExtrudeSplineCoroutine()
    {
        yield return new WaitForSeconds(traceInterval);
        ExtrudeMesh();
    }

    void ExtrudeMesh()
    {
        if (splineExtrude == null)
        {
            Debug.Log("Spline extrude is null");
            return;
        }
        Vector2 splinePercentage = splineExtrude.Range;

        if (splinePercentage.y >= 1)
        {
            StopCoroutine(ExtrudeSplineCoroutine());
            GameManager.onFinishPathTracing?.Invoke();
            return;
        }
        splinePercentage.y += traceSpeed;
        splineExtrude.Range = splinePercentage;
        splineExtrude.Rebuild();
        StartCoroutine(ExtrudeSplineCoroutine());
    }

}
