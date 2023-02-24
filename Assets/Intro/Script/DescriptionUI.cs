using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionUI : MonoBehaviour
{
    private Animator anim;

    void Awake() => anim = GetComponent<Animator>();

    public void CloseDes() => StartCoroutine(CloseAfterDelay());

    private IEnumerator CloseAfterDelay()
    {
        anim.SetTrigger("Close");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        anim.ResetTrigger("Close");
    }
}
