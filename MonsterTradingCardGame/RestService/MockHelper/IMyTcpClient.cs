using System;
using System.Collections.Generic;
using System.Text;

namespace Restservice.MockHelper
{
    public interface IMyTcpClient
    {
        public IMyNetWorkStream GetStream();
        public void Close();
    }
}
