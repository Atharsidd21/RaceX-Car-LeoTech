using UnityEngine;

public class TrackProgress : MonoBehaviour
{
    public Transform[] trackPoints; // ordered waypoints around track

    public float GetProgress(Vector3 carPosition)
    {
        float totalLength = 0f;
        float progress = 0f;

        for (int i = 0; i < trackPoints.Length - 1; i++)
        {
            float segmentLength = Vector3.Distance(
                trackPoints[i].position,
                trackPoints[i + 1].position
            );

            totalLength += segmentLength;

            float distanceToSegment =
                Vector3.Distance(carPosition, trackPoints[i].position);

            if (distanceToSegment <= segmentLength)
            {
                progress += distanceToSegment;
                break;
            }

            progress += segmentLength;
        }

        return progress / totalLength; // 0 ? 1
    }
}
