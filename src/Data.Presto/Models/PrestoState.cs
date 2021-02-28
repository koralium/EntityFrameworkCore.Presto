using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Models
{
    enum PrestoState
    {
        Queued,
        Planning,
        Starting,
        Running,
        Finished,
        Canceled,
        Failed
    }
}
