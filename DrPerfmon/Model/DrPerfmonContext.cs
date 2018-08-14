using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrPerfmon.Model
{
    public class DrPerfmonContext : DbContext
    {
        public DrPerfmonContext() : base("DefaultConnection")
        { }
        public DbSet<PerformanceCounterModel> PerformanceCounterModels { get; set; }
    }
}