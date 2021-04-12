using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTreeWebApi.ServiceModels
{
    public class TreeNodeException : ArgumentException
    {
        public TreeNodeException(string message)
            : base(message)
        { }
    }
}
