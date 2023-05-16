using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.Data;

namespace ProjectManagerContext.Data
{
    public class Variation
    {
        public Guid Id { get; set; }
        public string Reference { get; set; }
        public DateTime? DateIssued { get; set; }

        public string VariationText { get; set; }
        public string Delay { get; set; }
        public string Charge { get; set; }

        public DateTime? Accepted { get; set; }
        public Guid? AcceptorId { get; set; }
        public virtual Contact Acceptor { get; set; }

        public string? RaisedById { get; set; }
        public UserProfile RaisedBy { get; set; }

        public Guid? OriginatorId { get; set; }
        public virtual Contact Originator { get; set; }
        
        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public Variation()
        {
            Id = Guid.NewGuid();
        }
    }
}
