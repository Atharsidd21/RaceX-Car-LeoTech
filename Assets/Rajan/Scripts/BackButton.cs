using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BackButton : MonoBehaviour
{
    public void BackToMainMenu()
    {
        StartCoroutine(ExitAndReturn());
    }

    System.Collections.IEnumerator ExitAndReturn()
    {
        MultiplayerExitHandler.RequestExit();

        //  Max wait 2 sec
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Main Menu");
    }
}