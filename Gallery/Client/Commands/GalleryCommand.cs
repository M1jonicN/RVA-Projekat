using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Commands
{
    public abstract class GalleryCommand : Command
    {
        public string PIB
        {
            get;set;
        }
    }
}
