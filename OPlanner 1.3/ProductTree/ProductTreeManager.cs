using ProductStudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Controls;

namespace PlannerNameSpace
{
    public class ProductTreeManager
    {
        ProductTree ProductTree;
        bool IsEnsureHostProductTreeInProgress;
        bool HasCacheBeenChecked;

        public ProductTreeManager()
        {
            ProductTree = null;
            IsEnsureHostProductTreeInProgress = false;
            HasCacheBeenChecked = false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the product tree is available - if false is returned, you should
        /// call EnsureHostProductTree, and then wait for IsTreeItemCollectionAvailable to 
        /// return true before you call PopulateTree.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsTreeItemCollectionAvailable
        {
            get
            {
                if (IsEnsureHostProductTreeInProgress)
                {
                    return false;
                }

                if (ProductTree == null && HasCacheBeenChecked == false)
                {
                    ProductTree = UncacheProductTree();
                    HasCacheBeenChecked = true;

                    if (ProductTree != null && ProductTree.Count > 0)
                    {
                        return true;
                    }

                }

                if (ProductTree != null && ProductTree.Count > 0)
                {
                    return true;
                }

                return false;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Populates the given TreeView control with all the product tree nodes for the host
        /// back-end store, if the nodes are already available.  
        /// 
        /// Note: You should first check IsTreeItemCollectionAvailable and call 
        /// EnsureHostProductTree as necessary before calling PopulateTree.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void PopulateTree(TreeView treeView)
        {
            if (ProductTree != null && ProductTree.Count > 0)
            {
                PopulateUITree(treeView.Items, ProductTree.Nodes[0]);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Internal worker for PopulateTree.
        /// </summary>
        //------------------------------------------------------------------------------------
        void PopulateUITree(ItemCollection items, ProductTreeNode node)
        {
            TreeViewItem newItem = new TreeViewItem();
            newItem.Header = node;
            items.Add(newItem);

            foreach (ProductTreeNode childNode in node.ChildNodes)
            {
                PopulateUITree(newItem.Items, childNode);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Checks to see if the product tree is loaded and ready, and if not, kicks off a
        /// background process to build it. Once the tree is ready, IsTreeItemCollectionAvailable
        /// will return true.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void EnsureHostProductTree()
        {
            if (!IsEnsureHostProductTreeInProgress)
            {
                if (ProductTree == null)
                {
                    ProductTree = UncacheProductTree();
                    if (ProductTree == null)
                    {
                        BeginBuildTaskTree();
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Kick-starts the process of building and caching the product task tree.
        /// </summary>
        //------------------------------------------------------------------------------------
        void BeginBuildTaskTree()
        {
            IsEnsureHostProductTreeInProgress = true;

            ProductTree = new ProductTree();
            Node rootNode = HostItemStore.Instance.GetProductTreeRootNode();
            BackgroundTask buildTreeTask = new BackgroundTask(false);
            buildTreeTask.DoWork += buildTreeTask_DoWork;
            buildTreeTask.TaskCompleted += buildTreeTask_TaskCompleted;
            buildTreeTask.RunTaskAsync();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Background worker for EnsureHostProductTree.
        /// </summary>
        //------------------------------------------------------------------------------------
        void buildTreeTask_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BuildProductTree(ProductTree, 0);
            CacheProductTree(ProductTree);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Completion handler for EnsureHostProductTree.
        /// </summary>
        //------------------------------------------------------------------------------------
        void buildTreeTask_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            IsEnsureHostProductTreeInProgress = false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Builds a list of product tree nodes that represents the full product tree for the
        /// host back-end item store.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void BuildProductTree(ProductTree productTree, int startingTreeID)
        {
            Node rootPSNode = HostItemStore.Instance.GetProductTreeRootNode();
            Node startingNode = rootPSNode.FindNodeInSubtree(startingTreeID);
            BuildProductTreeWorker(productTree.Nodes, startingNode);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Worker method for BuildProductTree.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void BuildProductTreeWorker(List<ProductTreeNode> nodes, Node psNode)
        {
            ProductTreeNode node = new ProductTreeNode(psNode);
            nodes.Add(node);

            foreach (Node childNode in psNode.Nodes)
            {
                BuildProductTreeWorker(node.ChildNodes, childNode);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Caches the given list of product tree nodes locally for quick access next time.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void CacheProductTree(ProductTree tree)
        {
            if (tree != null && tree.Count > 0)
            {
                IFormatter formatter = new BinaryFormatter();
                string fullPath = GetProductTreeCacheFullPath();
                Stream stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, tree);
                stream.Close();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Uncaches the last list of product tree nodes that was cached with CachProductTree.
        /// If there is no valid cache available, null will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static ProductTree UncacheProductTree()
        {
            string fullPath = GetProductTreeCacheFullPath();
            if (System.IO.File.Exists(fullPath))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                try
                {
                    ProductTree tree = (ProductTree)formatter.Deserialize(stream);
                    if (tree != null)
                    {
                        // If the help manager indicates that the host product tree has been updated in
                        // the back-end store since the last time the tree was cached, invalidate it so
                        // that it will be re-cached.
                        DateTime? lastProductTreeUpdate = HelpManager.Instance.HostProductTreeLastUpdate;
                        if (lastProductTreeUpdate != null)
                        {
                            DateTime lastCached = tree.CachedDateTime;
                            if (lastProductTreeUpdate.Value >= lastCached)
                            {
                                return null;
                            }
                        }
                    }

                    return tree;
                }

                // If the tree couldn't be uncached for any reason, return null to trigger a rebuild.
                catch (Exception e)
                {
                    Globals.ApplicationManager.HandleException(e);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the full path to an appropriate local file for the caching of the built
        /// product tree.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string GetProductTreeCacheFullPath()
        {
            string rootFolder = ApplicationManager.GetOPlannerFolder();
            string treeCacheFolder = Path.Combine(rootFolder, HostItemStore.Instance.StoreName);
            if (!System.IO.Directory.Exists(treeCacheFolder))
            {
                System.IO.Directory.CreateDirectory(treeCacheFolder);
            }

            return Path.Combine(treeCacheFolder, "ProductTreeCache.bin");
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Selects the node in the given TreeView control that corresponds to the given
        /// treeId, and expands the tree to that node.  This TreeView must be populated using 
        /// the BindFeatureAreaTree for this selection to work.
        /// </summary>
        //------------------------------------------------------------------------------------
        public TreeViewItem SelectFeatureAreaTreeNode(System.Windows.Controls.TreeView treeView, int treeId)
        {
            foreach (TreeViewItem item in treeView.Items)
            {
                TreeViewItem selectedItem = SelectTreeViewItem(treeView, item, treeId);
                if (selectedItem != null)
                {
                    return selectedItem;
                }
            }

            return null;
        }

        public int GetSelectedAreaTreeID(TreeView treeView)
        {
            ProductTreeNode selectedNode = GetTreeNodeFromTreeViewItem(treeView.SelectedItem as TreeViewItem);
            return selectedNode == null ? -1 : selectedNode.TreeID;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Private helper function for SelectFeatureAreaTreeNode.
        /// </summary>
        //------------------------------------------------------------------------------------
        private TreeViewItem SelectTreeViewItem(System.Windows.Controls.TreeView treeView, TreeViewItem item, int treeId)
        {
            ProductTreeNode treeNode = GetTreeNodeFromTreeViewItem(item);
            if (treeNode.TreeID == treeId)
            {
                ExpandTreeViewItem(item);
                item.Focus();
                item.BringIntoView();
                item.IsSelected = true;
                return item;
            }

            foreach (TreeViewItem childItem in item.Items)
            {
                TreeViewItem selectedItem = SelectTreeViewItem(treeView, childItem, treeId);
                if (selectedItem != null)
                {
                    return selectedItem;
                }
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Another helper function for binding the tree.
        /// </summary>
        //------------------------------------------------------------------------------------
        public ProductTreeNode GetTreeNodeFromTreeViewItem(TreeViewItem treeViewItem)
        {
            return treeViewItem == null ? null : treeViewItem.Header as ProductTreeNode;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Expands the node of the given TreeViewItem, and all its parents.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void ExpandTreeViewItem(TreeViewItem item)
        {
            List<TreeViewItem> parentItems = new List<TreeViewItem>();
            TreeViewItem parent = item.Parent as TreeViewItem;
            while (parent != null)
            {
                parentItems.Insert(0, parent);
                parent = parent.Parent as TreeViewItem;
            }

            foreach (TreeViewItem parentItem in parentItems)
            {
                parentItem.IsExpanded = true;
            }
        }

    }
}
