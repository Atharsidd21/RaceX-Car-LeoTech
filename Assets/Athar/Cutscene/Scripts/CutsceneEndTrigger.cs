using UnityEngine;
using UnityEngine.Playables;

public class CutsceneEndTrigger : MonoBehaviour
{
    public PlayableDirector director;

    private void Start()
    {
        director.stopped += OnCutsceneStopped;
    }

    void OnCutsceneStopped(PlayableDirector obj)
    {
       // GameManager.Instance.OnCutsceneFinished();
    }
}