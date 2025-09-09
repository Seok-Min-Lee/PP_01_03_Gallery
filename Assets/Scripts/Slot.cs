using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum SlotState
{
    Unavailable,
    isUsing,
    available,
}
public class Slot : MonoBehaviour
{
    public RawImage defaultImage;
    public RawImage rawImage;
    public RawImage tempImage;

    public SlotState state { get { return _state; } }
    public float timer { get { return _timer; } }

    public EditorDataRaw editorDataRaw { get; private set; }
    public int timeLimit { get; private set; }

    private SlotState _state = SlotState.available;
    private float _timer = 0f;
    private void Awake()
    {
        timeLimit = 10;
    }
    private void Update()
    {
        if (_state == SlotState.isUsing)
        {
            if (_timer > timeLimit)
            {
                Deactivate();
            }

            _timer += Time.deltaTime;
        }
    }
    public void Activate(EditorDataRaw editorDataRaw)
    {
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            _state = SlotState.Unavailable;
            _timer = 0f;
            this.editorDataRaw = editorDataRaw;
            rawImage.texture = ConvertBytesToTexture(editorDataRaw.Texture);

            yield return ShowCor(rawImage.GetComponent<CanvasGroup>());

            _state = SlotState.isUsing;
        }
    }
    public void Change(EditorDataRaw editorDataRaw)
    {
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            _state = SlotState.Unavailable;
            _timer = 0f;

            tempImage.gameObject.SetActive(true);
            tempImage.GetComponent<CanvasGroup>().alpha = 0f;
            tempImage.texture = ConvertBytesToTexture(editorDataRaw.Texture);

            yield return ShowCor(tempImage.GetComponent<CanvasGroup>());

            this.editorDataRaw = editorDataRaw;
            rawImage.texture = ConvertBytesToTexture(editorDataRaw.Texture);
            tempImage.gameObject.SetActive(false);

            _state = SlotState.isUsing;
        }
    }
    private void Deactivate()
    {
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            _state = SlotState.Unavailable;
            _timer = 0f;

            yield return HideCor(rawImage.GetComponent<CanvasGroup>());

            editorDataRaw = null;
            rawImage.texture = null;
            _state = SlotState.available;
        }
    }

    IEnumerator ShowCor(CanvasGroup canvasGroup)
    {
        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t;

            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
    IEnumerator HideCor(CanvasGroup canvasGroup)
    {
        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1 - t;

            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
    private Texture2D ConvertBytesToTexture(byte[] bytes)
    {
        Texture2D tex = new Texture2D(0, 0);
        tex.LoadImage(bytes);

        return tex;
    }
}
