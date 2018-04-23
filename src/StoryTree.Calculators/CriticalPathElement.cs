using StoryTree.Data;
using StoryTree.Data.Tree;

namespace StoryTree.Gui.Converters
{
    public class CriticalPathElement
    {
        public CriticalPathElement(TreeEvent treeEvent, FragilityCurve fragilityCurve, bool failElement)
        {
            Element = treeEvent;
            FragilityCurve = fragilityCurve;
            ElementFails = failElement;
        }

        public TreeEvent Element { get; }

        public bool ElementFails { get; }

        public FragilityCurve FragilityCurve { get; }
    }
}