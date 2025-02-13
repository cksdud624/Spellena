using UnityEngine;

namespace BehaviorTree
{
    public class Tree : MonoBehaviour
    {
        protected Node root = null;
        protected virtual void Start()
        {
            root = SetupTree();
        }
        protected virtual void Update()
        {
            if (root != null) root.Evaluate();
        }
        protected virtual Node SetupTree() 
        {
            return root;
        }
    }
}
