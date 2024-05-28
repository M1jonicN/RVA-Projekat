using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface ILogInterface
    {
        void Log(string username, string message);
    }
}
