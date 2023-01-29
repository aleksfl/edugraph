using System;
using System.Linq;

namespace EdugraphCore {

    public enum NodeType {
        Subject,
        Knowledge

    }

    public class Node {

        // Either automatically set to an int, or to a subject code manually.
        public string Id { get; set; }
        public string Name { get; set; }
        public NodeType Type { get; set; }

        // Check if id is a subject code, which implies that the node represents a subject.
        public NodeType CheckType { get {
                if (Id.Any(a => char.IsLetter(a))) {
                    return NodeType.Subject;
                } else {
                    return NodeType.Knowledge;
                }
            } }
    }
}
