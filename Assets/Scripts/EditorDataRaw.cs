using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class EditorDataRaw
{
    public EditorDataRaw(
        int id, 
        int password, 
        int filterNo, 
        bool isDisplayed, 
        string registerDateTime, 
        string displayDateTime, 
        int studioId, 
        byte[] textureRaw
    )
    {
        this.id = id;
        this.password = password;
        this.filterNo = filterNo;
        this.isDisplayed = isDisplayed;
        this.registerDateTime = registerDateTime;
        this.displayDateTime = displayDateTime;
        this.studioId = studioId;
        this.textureRaw = textureRaw;
    }
    public int Id => id;
    public int Password => password;
    public int FilterNo => filterNo;
    public bool IsDisplayed => isDisplayed;
    public string RegisterDateTime => registerDateTime;
    public string DisplayDateTime => displayDateTime;
    public int StudioId => studioId;
    public byte[] Texture => textureRaw;
    [SerializeField] private int id;
    [SerializeField] private int password;
    [SerializeField] private int filterNo;
    [SerializeField] private bool isDisplayed;
    [SerializeField] private string registerDateTime;
    [SerializeField] private string displayDateTime;
    [SerializeField] private int studioId;
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
            isDisplayed: isDisplayed,
            registerDateTime: registerDateTime,
            displayDateTime: displayDateTime,
            studioId: studioId,
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
            bool isDisplayed, 
            string registerDateTime, 
            string displayDateTime, 
            int studioId, 
            int textureLength
        )
        {
            this.id = id;
            this.password = password;
            this.filterNo = filterNo;
            this.isDisplayed = isDisplayed;
            this.registerDateTime = registerDateTime;
            this.displayDateTime = displayDateTime;
            this.studioId = studioId;
            this.textureLength = textureLength;
        }
        public int Id => id;
        public int Password => password;
        public int FilterNo => filterNo;
        public bool IsDisplayed => isDisplayed;
        public string RegisterDateTime => registerDateTime;
        public string DisplayDateTime => displayDateTime;
        public int StudioId => studioId;
        public int TextureLength => textureLength;

        [SerializeField] private int id;
        [SerializeField] private int password;
        [SerializeField] private int filterNo;
        [SerializeField] private bool isDisplayed;
        [SerializeField] private string registerDateTime;
        [SerializeField] private string displayDateTime;
        [SerializeField] private int studioId;
        [SerializeField] private int textureLength;
    }
}
