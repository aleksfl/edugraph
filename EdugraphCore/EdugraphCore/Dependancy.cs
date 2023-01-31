using System;
using System.Linq;

namespace EdugraphCore {

    public enum RelationType {
        DependantOn,
        HelpedBy,
        PartOf,

    }

    // Can represent a direct Relation between nodes, or part of a multi node chain.
    public class Relation {
        
        // Unique for each Relation
        public string Id { get; set; }

        // There can be multiple dependancies with the same dependant and provider ids.
        public string DependantNodeId { get; set; }
        public string ProviderNodeId { get; set; }
        public RelationType Type { get; set; }

        // The Nodes that this relation is actually about, it will be the last node when acquiring a potential chain of nodes.
        public string RelationNodeId { get; set; }
        // Is this a direct Relation or part of a multi node chain?
        public bool IsDirectRelation { get {
                return ProviderNodeId == RelationNodeId;
            }            
        }
        // More flexible way to describe the relation than just the type, can be just about anything.
        public string Description { get; set; }
        
    }
}
