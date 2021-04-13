using System;

namespace TestTreeWebApi.ServiceModels
{
    public class TreeNodeCreateException : Exception
    {
        public TreeNodeCreateException(string message)
            : base(message)
        { }


    }
}
