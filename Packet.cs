using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

//Written by Abdul Galeel Ali

namespace PacketModule
{
    public enum ClientPackets
    {
        welcome,
        joinLobby,
        createLobby,
        displayLobbies,
        createDisplay,
        changeDisplay,
        deleteDisplay,
        otherDisplays,
        chat,
        startMatch,
        createUnit,
        moveUnit,
        unitAttack, 
        nextTurn,
        updateResources,
        startBuild,
        userLeave,
        removeForest,
        setHost,
        win
    }

    public enum ViewTypes
    {
        empty,
        gridPos,
        unit,
        building
    }

    public enum Resources
    {
        Ore,
        Wood,
        Gold
    }

    public class Packet : IDisposable
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

        public Packet()
        {
            buffer = new List<byte>(); 
            readPos = 0;
        }

        public Packet(int _id)
        {
            buffer = new List<byte>();
            readPos = 0; 

            Write(_id); 
        }

        public Packet(byte[] _data)
        {
            buffer = new List<byte>();
            readPos = 0;

            SetBytes(_data);
        }

        public void SetBytes(byte[] _data)
        {
            Write(_data);
            readableBuffer = buffer.ToArray();
        }

        public void WriteLength()
        {
            buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count)); 
        }

        public void InsertInt(int value)
        {
            buffer.InsertRange(0, BitConverter.GetBytes(value)); 
        }

        public byte[] ToArray()
        {
            readableBuffer = buffer.ToArray();
            return readableBuffer;
        }

        public int Length()
        {
            return buffer.Count; 
        }

        public int UnreadLength()
        {
            return Length() - readPos;
        }

        public void Reset(bool _shouldReset = true)
        {
            if (_shouldReset)
            {
                buffer.Clear(); 
                readableBuffer = null;
                readPos = 0;
            }
            else
            {
                readPos -= 4; 
            }
        }

        public void Write(byte value)
        {
            buffer.Add(value);
        }

        public void Write(byte[] value)
        {
            buffer.AddRange(value);
        }

        public void Write(short value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(int value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(long value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(float value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(bool value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(string value)
        {
            Write(value.Length); 
            buffer.AddRange(Encoding.ASCII.GetBytes(value));
        }

        public void Write(Vector3 value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
        }

        public void Write(Quaternion value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
            Write(value.w);
        }

        public void Write(Color color)
        {
            Write(color.r);
            Write(color.g);
            Write(color.b);
            Write(color.a);
        }
        public void Write(Color[] colors)
        {
            Write(colors.Length);
            for (int i = 0; i < colors.Length; i++)
            {
                Write(colors[i]);
            }
        }

        public void Write(string[] value)
        {
            Write(value.Length); 
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }
        public Color[] ReadColorArray(bool moveReadPos = true)
        {
            int length = ReadInt(moveReadPos);
            Color[] colors = new Color[length];

            for (int i = 0; i < length; i++)
            {
                colors[i] = ReadColor(moveReadPos);
            }

            return colors;
        }

        public string[] ReadStrArray(bool moveReadPos = true)
        {
            int length = ReadInt(moveReadPos);
            string[] strArray = new string[length];

            for (int i = 0; i < length; i++)
            {
                strArray[i] = ReadString(moveReadPos);
            }

            return strArray;
        }

        public void Write(int[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }

        public int[] ReadIntArray(bool moveReadPos = true)
        {
            int length = ReadInt(moveReadPos);
            int[] intArray = new int[length];

            for (int i = 0; i < length; i++)
            {
                intArray[i] = ReadInt(moveReadPos);
            }

            return intArray;
        }

        public byte ReadByte(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte value = readableBuffer[readPos]; 
                if (moveReadPos)
                {
                    readPos += 1; 
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        public byte[] ReadBytes(int _length, bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte[] value = buffer.GetRange(readPos, _length).ToArray(); 
                if (moveReadPos)
                {
                    readPos += _length;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        public short ReadShort(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                short value = BitConverter.ToInt16(readableBuffer, readPos); 
                if (moveReadPos)
                {
                    readPos += 2; 
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }

        public int ReadInt(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                int value = BitConverter.ToInt32(readableBuffer, readPos);
                if (moveReadPos)
                {
                    readPos += 4; 
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }

        public long ReadLong(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                long value = BitConverter.ToInt64(readableBuffer, readPos); 
                if (moveReadPos)
                {
                    readPos += 8;
                }
                return value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        public float ReadFloat(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                float value = BitConverter.ToSingle(readableBuffer, readPos); 
                if (moveReadPos)
                {
                    readPos += 4; 
                }
                return value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }

        public bool ReadBool(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                bool value = BitConverter.ToBoolean(readableBuffer, readPos); 
                if (moveReadPos)
                {
                    readPos += 1;
                }
                return value; 
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        public string ReadString(bool moveReadPos = true)
        {
            try
            {
                int _length = ReadInt(); 
                string value = Encoding.ASCII.GetString(readableBuffer, readPos, _length); 
                if (moveReadPos && value.Length > 0)
                {
                    readPos += _length;
                }
                return value; 
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        public Color ReadColor(bool moveReadPos = true)
        {
            return new Color(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
        }

        public Vector3 ReadVector3(bool moveReadPos = true)
        {
            return new Vector3(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
        }

        public Quaternion ReadQuaternion(bool moveReadPos = true)
        {
            return new Quaternion(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
        }

        private bool disposed = false;

        protected virtual void Dispose(bool _disposing)
        {
            if (!disposed)
            {
                if (_disposing)
                {
                    buffer = null;
                    readableBuffer = null;
                    readPos = 0;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}