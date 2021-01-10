using System;
using System.Collections.Generic;
using System.Text;

namespace Restservice.MockHelper
{
    public interface IMyNetWorkStream
    {
       int Read(byte[] buffer, int offset, int size);
        bool DataAvailable { get; }
        void Write(byte[] buffer, int offset, int size);
    }
}
