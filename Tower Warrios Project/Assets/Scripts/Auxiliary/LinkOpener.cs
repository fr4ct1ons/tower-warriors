using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class LinkOpener : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI target;
    [SerializeField] private Camera mainCam;

    private void Awake()
    {
        if (!mainCam)
            mainCam = Camera.main;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(target, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1) //A link was clicked.
        {
            TMP_LinkInfo linkInfo = target.textInfo.linkInfo[linkIndex];
            Debug.Log($"Link: {linkInfo.GetLinkID()}");
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}
