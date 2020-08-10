using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Models
{
    public enum PrestoState
    {
        QUEUED,
        RUNNING,
        PLANNING,
        STARTING,
        FINISHED,
        FAILED,
        FINISHING
    }
}
