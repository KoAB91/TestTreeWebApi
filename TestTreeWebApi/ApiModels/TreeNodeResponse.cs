using System.Collections.Generic;
using TestTreeWebApi.ServiceModels;

namespace TestTreeWebApi.ApiModels
{
    public class TreeNodeResponse
    {
        public string Name { get; set; }
        public List<TreeNodeDTO> Childs { get; set; }

        public TreeNodeResponse(TreeNodeDTO treeNodeDTO)
        {
            Name = treeNodeDTO.Name;
            Childs = treeNodeDTO.Childs;
        }
    }
}
