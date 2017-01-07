using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class Hashmap : IClosedSet
    {
        private Dictionary<NodeRecord, NodeRecord> NodeRecords { get; set; }

        public Hashmap()
        {
            this.NodeRecords = new Dictionary<NodeRecord, NodeRecord>();
        }

        public void Initialize()
        {
            this.NodeRecords.Clear();
        }

        public void AddToClosed(NodeRecord nodeRecord)
        {
            this.NodeRecords.Add(nodeRecord, nodeRecord);
        }

        public void RemoveFromClosed(NodeRecord nodeRecord)
        {
            this.NodeRecords.Remove(nodeRecord);
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            if (!this.NodeRecords.ContainsKey(nodeRecord))
            {
                return null;
            }
            return this.NodeRecords[nodeRecord];
        }

        public ICollection<NodeRecord> All()
        {
            return this.NodeRecords.Values;
        }

    }
}
