using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ctrl_Main : MonoBehaviour
{
    [SerializeField] private Slot[] slots;

    private Queue<EditorDataRaw> dataQueue = new Queue<EditorDataRaw>();
    private int slotIndex = 0;
    private void Start()
    {
        Debug.Log("Client is Available? " + (Client.Instance != null));
        RequestUndisplayedIdList();
    }

    float timer = 0f;
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > 3f)
        {
            if (dataQueue.Count > 0)
            {
                Display();
            }

            timer = 0f;
        }
    }
    public void RequestUndisplayedIdList()
    {
        Client.Instance.RequestGetUndisplayedIdList();
    }
    private void Display()
    {
        if (slotIndex == slots.Length)
        {
            slotIndex = 0;
        }
        Slot slot = slots[slotIndex++];

        EditorDataRaw editorDataRaw = dataQueue.Dequeue();
        if (slot.state == SlotState.available)
        {
            slot.Activate(editorDataRaw);
        }
        else
        {
            slot.Change(editorDataRaw);
        }

        Client.Instance.RequestUpdateDisplayStateById(editorDataRaw.Id);
    }

    public void Add(EditorDataRaw editorDataRaw)
    {
        dataQueue.Enqueue(editorDataRaw);
    }
    public void CallRequestData(IEnumerable<int> ids)
    {
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            for (int i = 0; i < ids.Count(); i++)
            {
                Client.Instance.RequestGetEditorDataById(ids.ElementAt(i));

                yield return new WaitForSeconds(1f);
            }
        }
    }
}
