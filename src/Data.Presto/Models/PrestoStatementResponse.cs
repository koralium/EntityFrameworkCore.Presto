using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Models
{
    internal class PrestoStatementResponse
    {
        public string NextUri { get; set; }

        public string InfoUri { get; set; }

        public string Id { get; set; }

        public PrestoStatsResponse Stats { get; set; }

        public List<List<object>> Data { get; set; }

        public List<PrestoColumn> Columns { get; set; }
    }
}
