using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class EditorDataRaw
{
    public enum State
    {
        Registered = 0,
        Released = 1,
        Displayed = 2
    }
    public EditorDataRaw(
        int id,
        int password,
        int filterNo,
        int stateNo,
        string registerDateTime,
        string releaseDateTime,
        string displayDateTime,
        byte[] textureRaw
    )
    {
        this.id = id;
        this.password = password;
        this.filterNo = filterNo;
        this.stateNo = stateNo;
        this.registerDateTime = registerDateTime;
        this.releaseDateTime = releaseDateTime;
        this.displayDateTime = displayDateTime;
        this.textureRaw = textureRaw;
    }
    public int Id => id;
    public int Password => password;
    public int FilterNo => filterNo;
    public int StateNo => stateNo;
    public string RegisterDateTime => registerDateTime;
    public string ReleaseDateTime => releaseDateTime;
    public string DisplayDateTime => displayDateTime;
    public byte[] Texture => textureRaw;
    [SerializeField] private int id;
    [SerializeField] private int password;
    [SerializeField] private int filterNo;
    [SerializeField] private int stateNo;
    [SerializeField] private string registerDateTime;
    [SerializeField] private string releaseDateTime;
    [SerializeField] private string displayDateTime;
    [NonSerialized] private byte[] textureRaw;
    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
    public byte[] ToBytes()
    {
        int textureLength = textureRaw != null ? textureRaw.Length : 0;

        Header headerObj = new Header(
            id: id,
            password: password,
            filterNo: filterNo,
            stateNo: stateNo,
            registerDateTime: registerDateTime,
            releaseDateTime: releaseDateTime,
            displayDateTime: displayDateTime,
            textureLength: textureLength
        );

        string headerJson = JsonUtility.ToJson(headerObj);
        byte[] headerBytes = Encoding.UTF8.GetBytes(headerJson);

        int headerLength = headerBytes.Length;
        byte[] headerLengthBytes = BitConverter.GetBytes(headerLength);

        byte[] packet = new byte[4 + headerLength + textureLength];

        Buffer.BlockCopy(headerLengthBytes, 0, packet, 0, 4);
        Buffer.BlockCopy(headerBytes, 0, packet, 4, headerLength);

        if (textureRaw.Length > 0)
        {
            Buffer.BlockCopy(textureRaw, 0, packet, 4 + headerLength, textureRaw.Length);
        }

        return packet;
    }

    [Serializable]
    public class Header
    {
        public Header(
            int id,
            int password,
            int filterNo,
            int stateNo,
            string registerDateTime,
            string releaseDateTime,
            string displayDateTime,
            int textureLength
        )
        {
            this.id = id;
            this.password = password;
            this.filterNo = filterNo;
            this.stateNo = stateNo;
            this.registerDateTime = registerDateTime;
            this.releaseDateTime = releaseDateTime;
            this.displayDateTime = displayDateTime;
            this.textureLength = textureLength;
        }
        public int Id => id;
        public int Password => password;
        public int FilterNo => filterNo;
        public int StateNo => stateNo;
        public string RegisterDateTime => registerDateTime;
        public string ReleaseDateTime => releaseDateTime;
        public string DisplayDateTime => displayDateTime;
        public int TextureLength => textureLength;

        [SerializeField] private int id;
        [SerializeField] private int password;
        [SerializeField] private int filterNo;
        [SerializeField] private int stateNo;
        [SerializeField] private string registerDateTime;
        [SerializeField] private string releaseDateTime;
        [SerializeField] private string displayDateTime;
        [SerializeField] private int textureLength;
    }
}
