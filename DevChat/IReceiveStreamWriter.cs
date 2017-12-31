using System;
using System.IO;

namespace DevChat
{
    public interface IReceiveStreamWriter
    {
        void SetStreamWriter(StreamWriter sw);
    }
}