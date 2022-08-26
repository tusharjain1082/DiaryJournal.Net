using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryJournal.Net
{
    // tree's node
    public class myTreeDomNode
    {
        public List<myTreeDomNode> children = new List<myTreeDomNode>();
        public myTreeDomNode? parent;
        public myNode? self;
        public UInt32 previousID = 0;
        //public bool isPurged = false;
    }

    public class myTreeDom
    {
        // entire tree structure
        public List<myTreeDomNode> tree = new List<myTreeDomNode>();

        // this method deletes the node by id
        public bool DeleteNodeRecursive(UInt32 id)
        { 
            myTreeDomNode? node = findNodeRecursive(id);
            if (node != null)
            {
                myTreeDomNode? parent = node.parent;
                if (parent == null)
                    return tree.Remove(node); // root node, remove from root of tree
                else
                    return parent.children.Remove(node); // a child node in some parent node, remove from parent.
            }
            // node not found, return error
            return false;
        }

        // this method finds the node by id
        public myTreeDomNode? findNodeRecursive(UInt32 id)
        {
            Queue<myTreeDomNode> queue = new Queue<myTreeDomNode>();

            // first enqueue all root nodes
            foreach (myTreeDomNode rootNode in tree)
            {
                if (rootNode.self.chapter.nodeID == id)
                    return rootNode;

                queue.Enqueue(rootNode);
            }

            while (queue.Count > 0)
            {
                myTreeDomNode currentNode = queue.Dequeue();

                if (currentNode.self.chapter.nodeID == id)
                    return currentNode;

                foreach (myTreeDomNode childNode in currentNode.children)
                {
                    if (childNode.self.chapter.nodeID == id)
                        return childNode;

                    queue.Enqueue(childNode);
                }

            }
            return null;
        }

        // this method sets the new id to a node. if demanded resets the children's parent node id to new parent id
        public void setNodeID(myTreeDomNode node, UInt32 newID, bool setChildrenParentID = true)
        {
            node.previousID = node.self.chapter.nodeID;
            node.self.chapter.nodeID = newID;
            if (setChildrenParentID)
            {
                foreach (myTreeDomNode childNode in node.children)
                    childNode.self.chapter.parentNodeID = newID;    
            }
        }

        // apply the set node to root nodes. this applies the set node to the entire 100% tree.
        public void applySetNode(ref myNode node)
        {
            myTreeDomNode setNode = new myTreeDomNode();
            setNode.self = node;
            setNode.parent = null;
            setNode.previousID = node.chapter.nodeID;
            foreach (myTreeDomNode rootNode in tree)
            {
                rootNode.parent = setNode;
                rootNode.self.chapter.parentNodeID = setNode.self.chapter.nodeID;   
                setNode.children.Add(rootNode); 
            }
            
            tree.Clear();
            tree.Add(setNode);
        }

        // apply the set node to root nodes. this applies the set node to the entire 100% tree.
        public void applySetNode(UInt32 id)
        {
            foreach (myTreeDomNode rootNode in tree)
                rootNode.self.chapter.parentNodeID = id;
        }

        // this method reindexes the tree with the given starting index and increments the index when it is applied
        public void reindexTree(ref UInt32 startingIndex)
        {
            Queue<myTreeDomNode> queue = new Queue<myTreeDomNode>();

            // first enqueue all root nodes
            foreach (myTreeDomNode rootNode in tree)
                queue.Enqueue(rootNode);

            while (queue.Count > 0)
            {
                myTreeDomNode currentNode = queue.Dequeue();

                UInt32 newIndex = entryMethods.CreateNodeID(ref startingIndex);
                setNodeID(currentNode, newIndex, true);

                foreach (myTreeDomNode childNode in currentNode.children)
                    queue.Enqueue(childNode);

            }

        }

        // this method nullifies special node type in entire tree
        public void nullSpecialNodeType()
        {
            Queue<myTreeDomNode> queue = new Queue<myTreeDomNode>();

            // first enqueue all root nodes
            foreach (myTreeDomNode rootNode in tree)
                queue.Enqueue(rootNode);

            while (queue.Count > 0)
            {
                myTreeDomNode currentNode = queue.Dequeue();

                foreach (myTreeDomNode childNode in currentNode.children)
                    queue.Enqueue(childNode);

                currentNode.self.chapter.specialNodeType = SpecialNodeType.None;
            }
        }

        // this method generates a list of entire tree
        public List<myTreeDomNode> ToList()
        {
            List<myTreeDomNode> list = new List<myTreeDomNode>();
            Queue<myTreeDomNode> queue = new Queue<myTreeDomNode>();

            // first enqueue all root nodes
            foreach (myTreeDomNode rootNode in tree)
                queue.Enqueue(rootNode);

            while (queue.Count > 0)
            {
                myTreeDomNode currentNode = queue.Dequeue();

                foreach (myTreeDomNode childNode in currentNode.children)
                    queue.Enqueue(childNode);

                list.Add(currentNode);
            }
            return list;
        }

        // this method builds the entire tree dom structure from a source nodes list.
        public void buildTree(ref List<myNode> srcNodes, bool sort = true, bool descending = false)
        {
            Queue<myTreeDomNode> queue = new Queue<myTreeDomNode>();
            tree.Clear();

            // first enqueue all root nodes
            foreach (myNode rootNode in entryMethods.findRootNodes(ref srcNodes, sort, descending))
            {
                myTreeDomNode node = new myTreeDomNode();
                node.self = rootNode;
                queue.Enqueue(node);
            }

            while (queue.Count > 0)
            {
                myTreeDomNode currentNode = queue.Dequeue();
                if (currentNode == null)
                    continue;

                // fetch this node's children
                List<myNode> children = entryMethods.findFirstLevelChildren(currentNode.self.chapter.nodeID, ref srcNodes, sort, descending);
                List<myTreeDomNode> myTreeDomNodeChildren = new List<myTreeDomNode>();

                foreach (myNode childNode in children)
                {
                    myTreeDomNode treeChildNode = new myTreeDomNode();
                    treeChildNode.self = childNode;
                    treeChildNode.parent = currentNode;
                    myTreeDomNodeChildren.Add(treeChildNode);
                    queue.Enqueue(treeChildNode);
                }
                // 1st in sequence is the parent node, so parent node is added 1st in sequence before all children.

                currentNode.children = myTreeDomNodeChildren;

                if (currentNode.parent == null)
                    tree.Add(currentNode);
            }
        }

        // this method builds a custom root nodes based tree chain through the list of all provided nodes.
        public void buildCustomTree(ref List<myNode> allNodes, ref List<myNode> rootNodes, bool sort = true, bool descending = false)
        {
            Queue<myTreeDomNode> queue = new Queue<myTreeDomNode>();
            tree.Clear();

            // sort if required
            if (sort)
                entryMethods.sortNodesByDateTime(ref rootNodes, descending);

            // first enqueue all root nodes
            foreach (myNode rootNode in rootNodes)
            {
                myTreeDomNode node = new myTreeDomNode();
                node.self = rootNode;
                tree.Add(node); // we add the root node direct
                queue.Enqueue(node);
            }

            while (queue.Count > 0)
            {
                myTreeDomNode currentNode = queue.Dequeue();
                if (currentNode == null)
                    continue;

                // fetch this node's children
                List<myNode> children = entryMethods.findFirstLevelChildren(currentNode.self.chapter.nodeID, ref allNodes, sort, descending);
                List<myTreeDomNode> myTreeDomNodeChildren = new List<myTreeDomNode>();

                foreach (myNode childNode in children)
                {
                    myTreeDomNode treeChildNode = new myTreeDomNode();
                    treeChildNode.self = childNode;
                    treeChildNode.parent = currentNode;
                    myTreeDomNodeChildren.Add(treeChildNode);
                    queue.Enqueue(treeChildNode);
                }
                // 1st in sequence is the parent node, so parent node is added 1st in sequence before all children.

                currentNode.children = myTreeDomNodeChildren;
            }
        }

    }
}
