using System.Collections.Generic;
using TestTreeWebApi.ApiModel;

namespace TestTreeWebApi.ServiceModel
{
    public class TreeNodeDTO
    {
        public string Name { get; set; }
        public List<TreeNodeDTO> Childs { get; set; }

        public TreeNodeDTO() { }
        public TreeNodeDTO(TreeNodeUpdateRequest request)
        {
            Name = request.Name;
        }

        public TreeNodeDTO(TreeNodeCreateRequest request)
        {
            Name = request.Name;
        }
    }
}
