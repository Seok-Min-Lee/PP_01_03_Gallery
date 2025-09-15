using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctrl_Main : MonoBehaviour
{
    [SerializeField] private Slot[] slots;
    private void Start()
    {
        Debug.Log("Client is Available? " + (Client.Instance != null));
        //for (int i = 1; i < 10; i++)
        //{
        //    samples.Enqueue(new EditorDataRaw(0, 0, 0, false, "", "", 0, System.IO.File.ReadAllBytes("C:/Users/dltjr/Desktop/새 폴더 (2)/" + i + ".jpeg")));
        //}
        //for (int i = 1; i < 10; i++)
        //{
        //    samples.Enqueue(new EditorDataRaw(0, 0, 0, false, "", "", 0, System.IO.File.ReadAllBytes("C:/Users/dltjr/Desktop/새 폴더 (2)/" + (10 - i) + ".jpeg")));
        //}
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RequestCount();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            RequestData();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Display();
        }
    }
    public void RequestCount()
    {
        Client.Instance.RequestGetUndisplayedCount();
    }
    private void RequestData()
    {
        Client.Instance.RequestGetEditorData();
    }
    private void Display()
    {
        if (slotIndex == slots.Length)
        {
            slotIndex = 0;
        }

        Slot slot = slots[slotIndex++];
        EditorDataRaw editorDataRaw = samples.Dequeue();

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
        samples.Enqueue(editorDataRaw);
    }
    public void AddJunk(int count)
    {
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            for (int i = 0; i < count; i++)
            {
                RequestData();

                yield return new WaitForSeconds(1f);
            }
        }
    }
    private Queue<EditorDataRaw> samples = new Queue<EditorDataRaw>();
    private int slotIndex = 0;
}
