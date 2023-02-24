using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    private Animator anim;
    public InputField PlayerNameInput;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void CloseSetting()
    {
        if(PlayerNameInput.text != "")
            StartCoroutine(CloseAfterDelay());
        else
        {
            PlayerNameInput.GetComponent<Animator>().SetTrigger("On");
            PlayerNameInput.GetComponent<Animator>().ResetTrigger("On");
        }
            

    }

    private IEnumerator CloseAfterDelay()
    {
        anim.SetTrigger("Close");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        anim.ResetTrigger("Close");
    }
}
