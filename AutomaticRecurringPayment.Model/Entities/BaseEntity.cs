using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Model.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime? InsertDateTime { get; set; }
        public string? InsertedBy { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
