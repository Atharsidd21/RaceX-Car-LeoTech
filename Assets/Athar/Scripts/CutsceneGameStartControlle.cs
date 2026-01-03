using UnityEngine;
using UnityEngine.Playables;

public class CutsceneGameStartController : MonoBehaviour
{
    public PlayableDirector director;

    private void Start()
    {
        // Play cutscene immediately
        director.Play();

        // Listen for cutscene end
        director.stopped += OnCutsceneFinished;
    }

    void OnCutsceneFinished(PlayableDirector obj)
    {
        // Tell GameManager to start countdown
        GameManager.Instance.StartRaceAfterCutscene();
    }
}
