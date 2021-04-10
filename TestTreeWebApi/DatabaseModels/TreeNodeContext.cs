using Microsoft.EntityFrameworkCore;

namespace TestTreeWebApi.DatabaseModels
{
    public class TreeNodeContext : DbContext
    {
        public TreeNodeContext(DbContextOptions<TreeNodeContext> options)
            : base(options)
        {
        }

        public DbSet<TreeNode> TreeNodes { get; set; }
    }
}
