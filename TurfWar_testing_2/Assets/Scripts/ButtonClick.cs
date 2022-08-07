using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _img;
    [SerializeField] private Sprite _default, _pressed;
    [SerializeField] private AudioClip _CompressClip, UncompressClip;
    [SerializeField] private AudioSource _Source;

    public void OnPointerDown (PointerEventData eventdata)
    {
        _img.sprite = _pressed;
    }

    public void OnPointerUp (PointerEventData eventData)
    {
        _img.sprite = _default;

    }
    public void IWasClicked()
    {
        Debug.Log("Clicked");

    }
}
