using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Client : MonoSingleton<Client>
{
    public Telepathy.Client client = new Telepathy.Client(1920 * 1080 + 1024);

    private Ctrl_Main ctrl => _ctrl ??= FindAnyObjectByType<Ctrl_Main>();
    private Ctrl_Main _ctrl;
    private void Awake()
    {
        // update even if window isn't focused, otherwise we don't receive.
        Application.runInBackground = true;

        // use Debug.Log functions for Telepathy so we can see it in the console
        Telepathy.Log.Info = Debug.Log;
        Telepathy.Log.Warning = Debug.LogWarning;
        Telepathy.Log.Error = Debug.LogError;

        // hook up events
        client.OnConnected = () => OnConnected();
        client.OnData = (message) => ReceiveMessage(message);
        client.OnDisconnected = () => Debug.Log("Client Disconnected");
    }
    private void Update()
    {
        if (client.Connected)
        {
            // tick to process messages
            // (even if not connected so we still process disconnect messages)
            client.Tick(1000);
        }
        else
        {
            client.Connect("127.0.0.1", 45604);
        }
    }

    private void OnApplicationQuit()
    {
        // the client/server threads won't receive the OnQuit info if we are
        // running them in the Editor. they would only quit when we press Play
        // again later. this is fine, but let's shut them down here for consistency
        client.Disconnect();
    }
    private void OnConnected()
    {
        Debug.Log("Client Connected");

        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter bw = new BinaryWriter(ms))
        {
            bw.Write(ConstantValues.CMD_REQUEST_CONNECT_GALLERY);

            client.Send(ms.ToArray());
        }
    }
    public void ReceiveMessage(ArraySegment<byte> message)
    {
        // clear previous message
        byte[] messageBytes = new byte[message.Count];
        for (int i = 0; i < messageBytes.Length; i++)
        {
            messageBytes[i] = message.Array[i];
        }

        byte[] commandBytes = new byte[4];
        Array.Copy(messageBytes, 0, commandBytes, 0, 4);
        int command = BitConverter.ToInt32(commandBytes);

        switch (command)
        {
            case ConstantValues.CMD_RESPONSE_GET_UNDISPLAYED_ID_LIST:
                ReceiveGetUndisplayedIdList(message: ref messageBytes);
                break;
            case ConstantValues.CMD_RESPONSE_GET_EDITOR_DATA:
                ReceiveGetEditorData(message: ref messageBytes);
                break;
            case ConstantValues.CMD_RESPONSE_UPDATE_DISPLAY_STATE:
                ReceiveUpdateDisplayState(message: ref messageBytes);
                break;
            default:
                break;
        }
    }

    public void RequestGetUndisplayedIdList()
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter bw = new BinaryWriter(ms))
        {
            bw.Write(ConstantValues.CMD_REQUEST_GET_UNDISPLAYED_ID_LIST);

            client.Send(ms.ToArray());
        }

        Debug.Log($"Request Get Undisplayed Id List");
    }
    public void RequestGetEditorDataById(int id)
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter bw = new BinaryWriter(ms))
        {
            bw.Write(ConstantValues.CMD_REQUEST_GET_EDITOR_DATA);
            bw.Write(id);

            client.Send(ms.ToArray());
        }

        Debug.Log($"Request Get Editor Data By Id::{id}");
    }
    public void RequestUpdateDisplayStateById(int id)
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter bw = new BinaryWriter(ms))
        {
            bw.Write(ConstantValues.CMD_REQUEST_UPDATE_DISPLAY_STATE);
            bw.Write(id);

            client.Send(ms.ToArray());
        }

        Debug.Log($"Request Update Display State By Id::{id}");
    }
    private void ReceiveGetUndisplayedIdList(ref byte[] message)
    {
        byte[] countBytes = new byte[4];
        Buffer.BlockCopy(message, 4, countBytes, 0, 4);
        int count = BitConverter.ToInt32(countBytes);

        if (count > 0)
        {
            List<int> ids = new List<int>();

            for (int i = 0; i < count; i++)
            {
                byte[] idBytes = new byte[4];
                Buffer.BlockCopy(message, 8 + i * 4, idBytes, 0, 4);
                int id = BitConverter.ToInt32(idBytes);

                ids.Add(id);
            }

            ctrl.CallRequestData(ids);
        }

        Debug.Log($"Receive Get Undisplayed Id List::{count}");
    }
    private void ReceiveGetEditorData(ref byte[] message)
    {
        // Receive Data
        byte[] headerLengthBytes = new byte[4];
        Buffer.BlockCopy(message, 4, headerLengthBytes, 0, 4);
        int headerLength = BitConverter.ToInt32(headerLengthBytes);

        byte[] headerBytes = new byte[headerLength];
        Buffer.BlockCopy(message, 8, headerBytes, 0, headerLength);
        string headerStr = Encoding.UTF8.GetString(headerBytes);

        // Data Parsing
        EditorDataRaw.Header header = JsonUtility.FromJson<EditorDataRaw.Header>(headerStr);

        byte[] textureBytes = new byte[header.TextureLength];
        Buffer.BlockCopy(message, 8 + headerLength, textureBytes, 0, header.TextureLength);

        EditorDataRaw raw = new EditorDataRaw(
            id: header.Id,
            password: header.Password,
            filterNo: header.FilterNo,
            stateNo: header.StateNo,
            registerDateTime: header.RegisterDateTime,
            releaseDateTime: header.ReleaseDateTime,
            displayDateTime: header.DisplayDateTime,
            textureRaw: textureBytes
        );

        Debug.Log($"Receive Get Editor Data::{raw.ToString()}");

        // Display UI
        ctrl.Add(raw);
    }
    private void ReceiveUpdateDisplayState(ref byte[] message)
    {
        byte[] idBytes = new byte[4];
        Buffer.BlockCopy(message, 4, idBytes, 0, 4);
        int id = BitConverter.ToInt32(idBytes);

        byte[] resultBytes = new byte[1];
        Buffer.BlockCopy(message, 8, resultBytes, 0, 1);
        bool result = BitConverter.ToBoolean(resultBytes);

        Debug.Log($"Receive Update Display State::{id}/{result}");
    }
}
