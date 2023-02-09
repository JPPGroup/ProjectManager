using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerContext.Data
{ public class Contact
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Company { get; set; }

        public required string Email { get; set; }
        public required string Position { get; set; }


        public ICollection<DrawingIssue> Issues { get; set; }
        public virtual ICollection<Variation> VariationsIssued { get; set; }
        public virtual ICollection<Variation> VariationsAccepted { get; set; }
    }
}
