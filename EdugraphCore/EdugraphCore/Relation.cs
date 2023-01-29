using System;
using System.Linq;

namespace EdugraphCore {

    public enum DependancyType {
        DependantOn,
        HelpedBy,
        PartOf,

    }

    // Can represent a direct dependancy between nodes, or part of a multi node chain.
    public class Dependancy {
        public string DependantNodeId { get; set; }
        public string ProviderNodeId { get; set; }
        public DependancyType Type { get; set; }
        public string DependancyObjectId { get; set; }
        // Is this a direct dependancy or part of a multi node chain?
        public bool IsDirectDependancy { get {
                return ProviderNodeId == DependancyObjectId;
            }            
        }
        
    }
}
