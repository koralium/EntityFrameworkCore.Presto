using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Models
{
    internal class PrestoStatsResponse
    {
        public long ProcessedBytes { get; set; }

        public long ProcessedRows { get; set; }

        public long WallTimeMillis { get; set; }

        public long CpuTimeMillis { get; set; }

        public long UserTimeMillis { get; set; }

        public PrestoState State { get; set; }


    }
}
