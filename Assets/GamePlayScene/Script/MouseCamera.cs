using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MouseCamera : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public float lookSensitivity;
    private float currentCameraRotationX = 0;
    private float currentCameraRotationY = 0;
    public float CameraRotationLimit;

    void Start()
    {
        

        lookSensitivity = -1f;
        CameraRotationLimit = 45f;
    }

    void Update()
    {
        if (PV.IsMine)
            CameraRotation();
    }

    void CameraRotation()
    {
        if (!GameManager.SettingKeyLock)
        {
            if (SitChiar.KeyLock == true)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                float xRotation = Input.GetAxisRaw("Vertical");
                float cameraRotationX = xRotation * -0.1f;
                currentCameraRotationX += cameraRotationX;

                float yRotation = Input.GetAxisRaw("Horizontal");
                float playerRotationY = yRotation * -0.1f;
                currentCameraRotationY += playerRotationY;

                currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -CameraRotationLimit, CameraRotationLimit);
                currentCameraRotationY = Mathf.Clamp(currentCameraRotationY, -CameraRotationLimit, CameraRotationLimit);

                gameObject.transform.localEulerAngles = new Vector3(currentCameraRotationX, -currentCameraRotationY, 0f);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                float xRotation = Input.GetAxisRaw("Mouse Y");
                float cameraRotationX = xRotation * -0.5f;
                currentCameraRotationX += cameraRotationX;

                currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -CameraRotationLimit, CameraRotationLimit);

                gameObject.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
            }
        }
    }
}