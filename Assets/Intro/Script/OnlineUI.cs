using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineUI : MonoBehaviour
{
    [SerializeField]
    private GameObject VideoPlayer;
    [SerializeField]
    private GameObject Sidebar;
    [SerializeField]
    private GameObject OnlineBackGround;
    [SerializeField]
    private GameObject MainIntroUI;
    [SerializeField]
    private GameObject BackButton;
    [SerializeField]
    private GameObject CreateRoomUI;
    [SerializeField]
    private GameObject CreateRoonUI_Img;

    private RectTransform OnlineBackRect;

    private Animator anim1;
    private Animator anim2;

    void OnEnable()
    {
        anim1 = OnlineBackGround.GetComponent<Animator>();
        anim2 = Sidebar.GetComponent<Animator>();
        OnlineBackRect = OnlineBackGround.GetComponent<RectTransform>();
        StartCoroutine(OnBackGra());
    }

    IEnumerator OnBackGra()
    {
        yield return new WaitForSeconds(1f);
        BackButton.GetComponent<Button>().enabled = true;
        VideoPlayer.SetActive(false);
    }

    public void Back_Main()
    {
        StartCoroutine(Off_Online());
    }

    IEnumerator Off_Online()
    {
        MainIntroUI.SetActive(true);
        VideoPlayer.SetActive(true);
        BackButton.GetComponent<Button>().enabled = false;
        anim1.SetTrigger("Close");
        yield return new WaitForSeconds(0.2f);
        anim2.SetTrigger("Close");        
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        anim1.ResetTrigger("Close");
        anim2.ResetTrigger("Close");
    }


    public void CreateRoom()
    {
        CreateRoomUI.SetActive(true);
    }

    public void CreateRoom_End() 
    {
        StartCoroutine(CreateRoomCor());
    }

    IEnumerator CreateRoomCor()
    {
        CreateRoonUI_Img.GetComponent<Animator>().SetTrigger("Off");
        yield return new WaitForSeconds(1.2f);
        CreateRoomUI.SetActive(false);
        CreateRoonUI_Img.GetComponent<Animator>().ResetTrigger("Off");
    }
}
