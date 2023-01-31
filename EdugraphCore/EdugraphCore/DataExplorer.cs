using System;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace EdugraphCore {
    public class Explorer {

        private class DataStore {
            public Dictionary<string, Node> Nodes { get; set; }
            public Dictionary<string, Relation> Relations { get; set; }
        }

        private Dictionary<string, Node> Nodes = new Dictionary<string, Node>();
        private Dictionary<string, Relation> Relations = new Dictionary<string, Relation>();
        private string directory = "data";
        // Commit changes first or they will be lost
        public void LoadDataFromFile() {
            var datapath = Directory.GetCurrentDirectory() + "/" + directory;
            var datafile = Directory.GetDirectories(datapath).Where(f => DateTime.TryParse(f, out _))
                .Select(f => DateTime.Parse(f)).OrderByDescending(f => f).First();
            var dataStr = File.ReadAllText(directory + "/" + datafile);
            var dataStoreFromJson = JsonSerializer.Deserialize<DataStore>(dataStr);
            Nodes = dataStoreFromJson.Nodes;
            Relations = dataStoreFromJson.Relations;
        }

        public Explorer(string dir = null) {
            if (dir != null) directory = dir;
            LoadDataFromFile(); // Try to populate with any and all data from newest file.
        }

        public bool IsNodeIndepedant(string nodeId) {
            return Relations.Values.Where(d => d.DependantNodeId == nodeId).Count() == 0;
        }

        // Returns list of nodes (by id) in Relation chain with the closest node first.
        // For single node chain the list will only contain a single node id
        public List<string> CalculateFullRelationPath(string relationId) {            
            if (!Relations.ContainsKey(relationId)) throw new Exception("Relation does not exist: " + relationId);
            var retval = new List<string>();
            var r = Relations[relationId];
            retval.Add(r.ProviderNodeId);
            // Not neccesary but makes code more readable
            if (r.IsDirectRelation) return retval;
            var workNodeId = r.ProviderNodeId;
            while (workNodeId != r.RelationNodeId) {
                var nextRelation = Relations.Values.Where(r => r.DependantNodeId == r.RelationNodeId).FirstOrDefault();
                if (nextRelation == null) throw new Exception("Incomplete dependancy chain for relation: " + relationId);
                workNodeId = nextRelation.ProviderNodeId;
                retval.Add(workNodeId);

            }
            return retval;
        }

        public void AddNode(string name, NodeType type, string id = null) {
            if (Nodes.ContainsKey(id)) throw new Exception("Node with id: " + id + " already exists");
            var now = DateTime.Now;
            // This looks weird but is just a crude way of getting a unique id
            if (id == null) id = (Nodes.Count().ToString() + now.Ticks.ToString());            
            Nodes.Add(id, new Node() {
                Id = id,
                Name = name,
                Type = type,
            });
        }

        public void RemoveNode(string id) {
            if (!Nodes.ContainsKey(id)) throw new Exception("Node with id: " + id + " does not exist");            
            Nodes.Remove(id);
            // Remove any relation to or from or regarding this node.
            var relationsToRemove = Relations.Values.Where(r => r.RelationNodeId == id ||
            r.ProviderNodeId == id || r.DependantNodeId == id).Select(r => r.Id);
            foreach(var r in relationsToRemove) {
                Relations.Remove(r);
            }
        }

        public void AddRelation(string dependantId, string providerId, RelationType type, string id = null,
            string relationNodeId = null, string description = null) {
            if (!Nodes.ContainsKey(dependantId)) throw new Exception("Node with id: " + dependantId + " does not exist");
            if (!Nodes.ContainsKey(providerId)) throw new Exception("Node with id: " + providerId + " does not exist");
            if (relationNodeId != null && !Nodes.ContainsKey(relationNodeId)) throw new Exception("Node with id: " + relationNodeId + " does not exist");
            var now = DateTime.Now;
            // This looks weird but is just a crude way of getting a unique id
            if (id == null) id = (Nodes.Count().ToString() + now.Ticks.ToString());
            var r = new Relation() {
                Id = id,
                DependantNodeId = dependantId,
                ProviderNodeId = providerId,
                Type = type            
            };
            if (relationNodeId != null) r.RelationNodeId = relationNodeId;
            if (description != null) r.Description = description;
            Relations.Add(id, r);
        }
        public void CommitChanges() {
            var store = new DataStore();
            store.Nodes = Nodes;
            store.Relations = Relations;
            string jsonData = JsonSerializer.Serialize(store);
            var datapath = Directory.GetCurrentDirectory() + "/" + directory;
            File.WriteAllText(datapath + "/" + DateTime.Now.ToString(), jsonData);
        }
    }
}
