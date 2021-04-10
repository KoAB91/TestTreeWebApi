using System.Collections.Generic;
using TestTreeWebApi.ServiceModel;

namespace TestTreeWebApi.ApiModel
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
