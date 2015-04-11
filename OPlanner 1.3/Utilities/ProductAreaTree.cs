using ProductStudio;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PlannerNameSpace
{
    public class ProductAreaTree
    {
        Node RootNode;

        public ProductAreaTree(Node rootNode)
        {
            RootNode = rootNode;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the complete product tree for the current product. The root will have 
        /// parent id = -200. The returned tree is a depth-first representation of the product 
        /// hierarchy.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void PopulateTree(TreeView treeView, int startingTreeID)
        {
            PopulateTreeCollection(treeView.Items, startingTreeID);
        }

        public bool IsItemUnderTreeID(int treeID, int itemTreeID)
        {
            if (treeID == itemTreeID)
            {
                return true;
            }

            Node startingNode = GetNodeFromTreeID(treeID);
            if (startingNode != null)
            {
                return IsItemUnderTreeNode(startingNode, itemTreeID);
            }

            return false;
        }

        private bool IsItemUnderTreeNode(Node node, int itemTreeID)
        {
            foreach (Node childNode in node.Nodes)
            {
                if (childNode.ID == itemTreeID)
                {
                    return true;
                }

                IsItemUnderTreeNode(childNode, itemTreeID);
            }

            return false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the complete product tree for the current product. The root will have 
        /// parent id = -200. The returned tree is a depth-first representation of the product 
        /// hierarchy.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void PopulateTreeCollection(ItemCollection items, int startingTreeID)
        {
            Node startingNode = GetNodeFromTreeID(startingTreeID);
            if (startingNode != null)
            {
                PopulateTree(items, startingNode);
            }
        }

        private void PopulateTree(ItemCollection items, Node node)
        {
            TreeViewItem newItem = new TreeViewItem();
            newItem.Header = new ProductTreeNode(node);
            items.Add(newItem);

            foreach (Node childNode in node.Nodes)
            {
                PopulateTree(newItem.Items, childNode);
            }
        }

        private Node GetNodeFromTreeID(int treeID)
        {
            return RootNode.FindNodeInSubtree(treeID);
        }

    }
}
