using ProductStudio;
using System;
using System.Collections.Generic;

namespace PlannerNameSpace
{
    [Serializable]
    public class ProductTreeNode
    {
        public ProductTreeNode(Node node)
        {
            if (node != null)
            {
                Name = node.Name;
                TreeID = node.ID;
                DisplayName = node.Name;
            }

            ChildNodes = new List<ProductTreeNode>();
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int TreeID { get; set; }
        public List<ProductTreeNode> ChildNodes { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public enum ProductTreeFormat
    {
        IncludeProduct,
        ExcludeProduct
    }
}
