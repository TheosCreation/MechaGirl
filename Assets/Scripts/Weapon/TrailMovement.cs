using System.Collections;
using UnityEngine;

public class TrailMovement : MonoBehaviour
{
    public Vector3 hitpoint;
    public Vector3 hitnormal;
    public float timeNeededToReach;
    public float timeToDestroyAfterReaching = 0.1f;
    public AnimationCurve curve;

    private TrailRenderer trailRenderer;

    private Vector3 startPoint;
    private float elapsedTime;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.Clear();
        startPoint = transform.position;

        trailRenderer.transform.position = startPoint;

        elapsedTime = 0f;
        StartCoroutine(MoveToPoint());
    }

    IEnumerator MoveToPoint()
    {
        while (elapsedTime < timeNeededToReach)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / timeNeededToReach;
            t = curve.Evaluate(t);
            transform.position = Vector3.Lerp(startPoint, hitpoint, t);
            yield return null;
        }
        transform.position = hitpoint;

        Destroy(gameObject, timeToDestroyAfterReaching);
    }
}
