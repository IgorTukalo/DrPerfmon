using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrPerfmon.Model
{
    public class PerformanceCounterModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string CounterName { get; set; }
        public string InstanceName { get; set; }
        public string MachineName { get; set; }
        public string CategoryNameRus { get; set; }
        public string CounterNameRus { get; set; }
    }
}
