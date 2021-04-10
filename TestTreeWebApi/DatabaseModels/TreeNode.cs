using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestTreeWebApi.DatabaseModels
{
    public class TreeNode
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public TreeNode Parent { get; set; }
        public string Name { get; set; }
        public ICollection<TreeNode> Childs { get; set; }

        public TreeNode()
        {
            Childs = new List<TreeNode>();
        }
    }
}
