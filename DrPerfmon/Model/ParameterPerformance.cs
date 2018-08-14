using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrPerfmon.Model
{
    [NotMapped]
    public class ParameterPerformance : PerformanceCounterModel
    {
        public double ValueCounter { get; set; }
        public DateTime TimeAdd { get; set; }
    }
}
