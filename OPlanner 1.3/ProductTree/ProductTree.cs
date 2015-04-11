using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    [Serializable]
    public class ProductTree
    {
        public DateTime CachedDateTime
        {
            get
            {
                return DateTime.FromBinary(binaryCachedDateTime);
            }
        }

        long binaryCachedDateTime { get; set; }
        public List<ProductTreeNode> Nodes { get; set; }
        public int Count { get { return Nodes.Count; } }

        public ProductTree()
        {
            Nodes = new List<ProductTreeNode>();
            binaryCachedDateTime = DateTime.Now.ToBinary();
        }
    }
}
