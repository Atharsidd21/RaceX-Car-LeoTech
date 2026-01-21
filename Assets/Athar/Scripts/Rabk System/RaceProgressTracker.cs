using UnityEngine;

public class RaceProgressTracker : MonoBehaviour
{
    public int currentLap = 0;
    public float trackProgress = 0f;
    public bool finished = false;

    private TrackProgress track;

    private void Start()
    {
        track = FindObjectOfType<TrackProgress>();
    }

    private void Update()
    {
        if (finished || track == null) return;

        trackProgress = track.GetProgress(transform.position);
    }
}
