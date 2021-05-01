using UnityEngine;
using UnityEngine.UI;
public class UIFx : MonoBehaviour
{
    public TweeningAnimator fxAnimator;
    private float timeSpend;

    private void Start()
    {
        timeSpend = 0;
        fxAnimator.GetReferences();
        fxAnimator.anim = Instantiate(fxAnimator.anim);
        StartCoroutine(fxAnimator.anim.Play(fxAnimator));
    }

    private void Update()
    {
        if (timeSpend > fxAnimator.anim.animationTime)
        {
            Destroy(gameObject);
        }
        timeSpend += Time.deltaTime;
    }
}
