
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MenuButtonSound : MonoBehaviour
{
    [Header("Quack Sound")]
    public AudioSource audioSource;
    public AudioClip quackClip;

    [Header("After Quack")]
    public UnityEvent onQuackFinished;

    private bool buttonPressed = false;

    public void PlayQuack()
    {
        if (buttonPressed)
            return;

        buttonPressed = true;
        StartCoroutine(QuackThenContinue());
    }

    private IEnumerator QuackThenContinue()
    {
        if (audioSource != null && quackClip != null)
        {
            audioSource.PlayOneShot(quackClip);
            yield return new WaitForSecondsRealtime(quackClip.length);
        }

        onQuackFinished?.Invoke();
    }
}
