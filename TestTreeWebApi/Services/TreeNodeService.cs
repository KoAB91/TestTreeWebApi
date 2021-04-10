using System.Collections.Generic;
using System.Linq;
using TestTreeWebApi.DatabaseModels;
using TestTreeWebApi.Enums;
using Microsoft.EntityFrameworkCore;
using TestTreeWebApi.ServiceModel;
using System.Text;

namespace TestTreeWebApi.Services
{
    public class TreeNodeService
    {
        private readonly TreeNodeContext _context;

        public TreeNodeService(TreeNodeContext context)
        {
            _context = context;
        }

        public List<TreeNodeDTO> GetAll()
        {
            var treeNodeList = _context.TreeNodes.Include(t => t.Childs);
            if(treeNodeList == null)
            {
                return null;
            }
            var treeNodeDTOList = new List<TreeNodeDTO>();
            foreach (TreeNode treeNode in treeNodeList)
            {
                AddAllChildrenTo(treeNode);
                treeNodeDTOList.Add(ConvertToDTO(treeNode));
            }
            return treeNodeDTOList;
        }

        public TreeNodeDTO GetById(long id)
        {
            var treeNode = _context.TreeNodes
                                    .Where(t => t.Id == id)
                                    .Include(t => t.Childs)
                                    .FirstOrDefault();
            if (treeNode == null)
            {
                return null;
            }
            AddAllChildrenTo(treeNode);
            return ConvertToDTO(treeNode);
        }

        
        public TreeNodeDTO GetByName(string name)
        {
            var treeNode = _context.TreeNodes
                                    .Where(t => t.Name.Equals(name))
                                    .Include(t => t.Childs)
                                    .FirstOrDefault();
            if(treeNode == null)
            {
                return null;
            }
            AddAllChildrenTo(treeNode);
            return ConvertToDTO(treeNode);
        }

        public string GetPath(string name)
        {
            var treeNode = _context.TreeNodes
                                    .Where(t => t.Name.Equals(name))
                                    .Include(t => t.Parent)
                                    .FirstOrDefault();
            if(treeNode == null)
            {
                return null;
            }
            if (!treeNode.ParentId.HasValue)
            {
                return treeNode.Name;
            }
            StringBuilder path = new StringBuilder(treeNode.Name);  
            while(treeNode != null && treeNode.ParentId.HasValue)
            {
                string parentName = treeNode.Parent.Name;
                // from current to root (business requirement)
                path.Append($@"\{parentName}") ;
                treeNode = _context.TreeNodes
                                    .Where(t => t.Name.Equals(parentName))
                                    .Include(t => t.Parent)
                                    .FirstOrDefault();
            }
            return path.ToString();
        }

        public TreeNodeDTO Update(string name, TreeNodeDTO treeNodeDTO)
        {
            var treeNode = _context.TreeNodes
                                    .Where(t => t.Name.Equals(name))
                                    .Include(t => t.Childs)
                                    .FirstOrDefault();
            if (treeNode == null)
            {
                return null;
            }
            treeNode.Name = treeNodeDTO.Name;
            _context.SaveChanges();
            AddAllChildrenTo(treeNode);
            return ConvertToDTO(treeNode);
        }

        public TreeNodeDTO Create(string name, TreeNodeDTO treeNodeDTO)
        {
            var treeNode = new TreeNode();
            treeNode.Name = treeNodeDTO.Name;
            if (name != null)
            {
                var parentTreeNode = _context.TreeNodes
                                                .Where(t => t.Name.Equals(name))
                                                .FirstOrDefault();
                if (parentTreeNode == null)
                {
                    return null;
                }
                treeNode.Parent = parentTreeNode;
                treeNode.ParentId = parentTreeNode.Id;
            }
            var entity =_context.TreeNodes.Add(treeNode);
            _context.SaveChanges();
            return ConvertToDTO(entity.Entity);
        }

        public DeleteResponse Delete(string name)
        {
            var treeNode = _context.TreeNodes
                                        .Where(t => t.Name.Equals(name))
                                        .Include(t => t.Childs)
                                        .FirstOrDefault();
            if (treeNode == null)
            {
                return DeleteResponse.NOT_FOUND;
            }
            RemoveBranchStartingFrom(treeNode);
            _context.SaveChanges();
            return DeleteResponse.ОК;
        }

        public void AddAllChildrenTo(TreeNode parentTreeNode)
        {
            if (parentTreeNode.Childs != null)
            {
                var childTreeNodes = new List<TreeNode>();
                foreach (TreeNode childTreeNode in parentTreeNode.Childs)
                {
                    var treeNode = _context.TreeNodes
                                                .Where(t => t.Name.Equals(childTreeNode.Name))
                                                .Include(t => t.Childs)
                                                .FirstOrDefault();
                    if (treeNode.Childs != null)
                    {
                        AddAllChildrenTo(treeNode);
                    }
                    childTreeNodes.Add(treeNode);
                }
                parentTreeNode.Childs = childTreeNodes;
            }
                
        }

        public void RemoveBranchStartingFrom(TreeNode parentTreeNode)
        {
            if (parentTreeNode.Childs != null)
            {
                foreach (TreeNode childTreeNode in parentTreeNode.Childs)
                {
                    var treeNode = _context.TreeNodes
                                                .Where(t => t.Name.Equals(childTreeNode.Name))
                                                .Include(t => t.Childs)
                                                .FirstOrDefault();
                    if (treeNode.Childs != null)
                    {
                        RemoveBranchStartingFrom(treeNode);
                    }
                    _context.TreeNodes.Remove(treeNode);
                }
            }
            _context.TreeNodes.Remove(parentTreeNode);
        }

        public TreeNodeDTO ConvertToDTO(TreeNode treeNode)
        {
            var treeNodeDTO = new TreeNodeDTO 
            {
                Name = treeNode.Name
            };
            if (treeNode.Childs != null)
            {
                var childs = new List<TreeNodeDTO>();
                foreach (TreeNode childTreeNode in treeNode.Childs)
                {
                    var child = ConvertToDTO(childTreeNode);
                    childs.Add(child);
                }
                treeNodeDTO.Childs = childs;
            }
            return treeNodeDTO;
        }
    }
}
