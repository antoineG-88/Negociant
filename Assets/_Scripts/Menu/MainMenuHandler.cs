using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    public SimpleButton playeButton;
    public SimpleButton tutorielButton;
    public TweeningAnimator blackScreenAnim;
    public int tutorielSceneIndex;
    public int mainSceneIndex;

    void Start()
    {
        blackScreenAnim.GetReferences();
        StartCoroutine(blackScreenAnim.anim.PlayBackward(blackScreenAnim, true));
    }

    void Update()
    {
        if(SaveLoader.I != null)
        {
            Destroy(SaveLoader.I.gameObject);
        }

        if(tutorielButton.isClicked)
        {
            playeButton.SetEnable(false);
            StartCoroutine(LoadScene(tutorielSceneIndex));
        }

        if (playeButton.isClicked)
        {
            tutorielButton.SetEnable(false);
            StartCoroutine(LoadScene(mainSceneIndex));
        }
    }

    private IEnumerator LoadScene(int index)
    {
        StartCoroutine(blackScreenAnim.anim.Play(blackScreenAnim));
        yield return new WaitForSeconds(blackScreenAnim.anim.animationTime);
        SceneManager.LoadScene(index);
    }
}
