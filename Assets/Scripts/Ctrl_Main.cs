using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctrl_Main : MonoBehaviour
{
    [SerializeField] private Slot[] slots;
    private void Start()
    {
        for (int i = 1; i < 10; i++)
        {
            samples.Enqueue(new EditorDataRaw(0, 0, 0, false, "", "", 0, System.IO.File.ReadAllBytes("C:/Users/dltjr/Desktop/새 폴더 (2)/" + i + ".jpeg")));
        }
        for (int i = 1; i < 10; i++)
        {
            samples.Enqueue(new EditorDataRaw(0, 0, 0, false, "", "", 0, System.IO.File.ReadAllBytes("C:/Users/dltjr/Desktop/새 폴더 (2)/" + (10 - i) + ".jpeg")));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (slotIndex == slots.Length)
            {
                slotIndex = 0;
            }

            Slot slot = slots[slotIndex++];
            if (slot.state == SlotState.available)
            {
                slot.Activate(samples.Dequeue());
            }
            else
            {
                slot.Change(samples.Dequeue());
            }
        }
    }

    private Queue<EditorDataRaw> samples = new Queue<EditorDataRaw>();
    private int slotIndex = 0;
}
