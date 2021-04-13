using System;

namespace TestTreeWebApi.ServiceModels
{
    public class TreeNodeMoveException : Exception
    {
        public TreeNodeMoveException(string message)
            : base(message)
        { }
    }
}
