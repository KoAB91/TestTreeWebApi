using System.Collections.Generic;
using System.Linq;
using TestTreeWebApi.DatabaseModels;
using TestTreeWebApi.Enums;
using Microsoft.EntityFrameworkCore;
using TestTreeWebApi.ServiceModels;
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

        public TreeNodeDTO UpdateName(string name, string newName)
        {
            var treeNode = _context.TreeNodes
                                    .Where(t => t.Name.Equals(name))
                                    .Include(t => t.Childs)
                                    .FirstOrDefault();
            if (treeNode == null)
            {
                return null;
            }
            treeNode.Name = newName;
            _context.SaveChanges();
            AddAllChildrenTo(treeNode);
            return ConvertToDTO(treeNode);
        }

        public TreeNodeDTO UpdateParent(string name, string parentName)
        {
            var treeNode = _context.TreeNodes
                                   .Where(t => t.Name.Equals(name))
                                   .Include(t => t.Childs)
                                   .Include(t => t.Parent)
                                   .FirstOrDefault();
            if (treeNode == null)
            {
                throw new TreeNodeCreateException($"No object with such name: \"{name}\"");
            }

            if (!treeNode.ParentId.HasValue)
            {
                throw new TreeNodeMoveException("Root movement is prohibited");
            }
            
            if(parentName != null)
            {
                var parentTreeNode = _context.TreeNodes
                                  .Where(t => t.Name.Equals(parentName))
                                  .FirstOrDefault();
                if (parentTreeNode == null)
                {
                    throw new TreeNodeCreateException($"No object with such name: \"{parentName}\"");
                }
                treeNode.ParentId = parentTreeNode.Id;
                treeNode.Parent = parentTreeNode;
            } else
            {
                var root = _context.TreeNodes
                                        .Where(t => !t.ParentId.HasValue)
                                        .FirstOrDefault();
                root.ParentId = treeNode.ParentId;
                root.Parent = treeNode;
                treeNode.ParentId = null;
                treeNode.Parent = null;
                treeNode.Childs.Add(root);
            }
           
            _context.SaveChanges();
            AddAllChildrenTo(treeNode);
            return ConvertToDTO(treeNode);
        }

        public TreeNodeDTO Create(TreeNodeDTO treeNodeDTO)
        {
            return Create(null, treeNodeDTO);
        }

        public TreeNodeDTO Create(string name, TreeNodeDTO treeNodeDTO)
        {
            if (!_context.TreeNodes.Any())
            {
                var root = new TreeNode();
                root.Name = treeNodeDTO.Name;
                root = _context.TreeNodes.Add(root)
                                                .Entity;
                _context.SaveChanges();
                return ConvertToDTO(root);
            }

            var treeNode = _context.TreeNodes
                                      .Where(t => t.Name.Equals(treeNodeDTO.Name))
                                      .FirstOrDefault();
            if (treeNode != null)
            {
                throw new TreeNodeCreateException("Object with such name is already created.");
            }

            treeNode = new TreeNode()
            {
                Name = treeNodeDTO.Name
            };

            
            if (name != null)
            {
                treeNode = AddToParentNode(name, treeNode);
            }
            else
            {
                treeNode = AddToRoot(treeNode);
            }
            return ConvertToDTO(treeNode);
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
            _context.TreeNodes.Remove(treeNode);
            _context.SaveChanges();
            return DeleteResponse.ОК;
        }

        public TreeNode AddToRoot(TreeNode treeNode)
        {
            treeNode = _context.TreeNodes.Add(treeNode)
                                                .Entity;
            var root = _context.TreeNodes
                                         .Where(t => !t.ParentId.HasValue)
                                         .FirstOrDefault();
            root.Parent = treeNode;
            root.ParentId = treeNode.Id;
            _context.SaveChanges();
            return treeNode;
        }

        public TreeNode AddToParentNode(string name, TreeNode treeNode)
        {
            var parentTreeNode = _context.TreeNodes
                                                .Where(t => t.Name.Equals(name))
                                                .FirstOrDefault();
            if (parentTreeNode == null)
            {
                throw new TreeNodeCreateException($"No object with such name: \"{name}\"");
            }
            treeNode.Parent = parentTreeNode;
            treeNode.ParentId = parentTreeNode.Id;
            treeNode = _context.TreeNodes.Add(treeNode)
                                                .Entity;
            _context.SaveChanges();
            return treeNode;
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
