using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Data
{
    public class Quote
    {
        public Guid Id { get; set; }

        public string IssuerId { get; set; } = null!;
        public virtual UserProfile Issuer { get; set; } = null!;

        public Guid ProjectId { get; set; }

        public DateTime Issued { get; set; }
        public DateTime? Responded { get; set; }

        public required bool Won { get; set; }

        public decimal TotalFee { get; set; }

        public Quote()
        {
            Id = Guid.NewGuid();
            Won = false;
            Responded = null;
        }

        public virtual Project Project { get; set; } = null!;
    }
}
