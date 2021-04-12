using System.Collections.Generic;
using TestTreeWebApi.ApiModels;

namespace TestTreeWebApi.ServiceModels
{
    public class TreeNodeDTO
    {
        public string Name { get; set; }

        public List<TreeNodeDTO> Childs { get; set; }
    }
}
