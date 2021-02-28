using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Client
{
    internal static class PrestoHeaders
    {
        public const string PRESTO_USER = "X-Presto-User";
        public const string PRESTO_SOURCE = "X-Presto-Source";
        public const string PRESTO_CATALOG = "X-Presto-Catalog";
        public const string PRESTO_SCHEMA = "X-Presto-Schema";
        public const string PRESTO_PATH = "X-Presto-Path";
        public const string PRESTO_TIME_ZONE = "X-Presto-Time-Zone";
        public const string PRESTO_LANGUAGE = "X-Presto-Language";
        public const string PRESTO_TRACE_TOKEN = "X-Presto-Trace-Token";
        public const string PRESTO_SESSION = "X-Presto-Session";
        public const string PRESTO_SET_CATALOG = "X-Presto-Set-Catalog";
        public const string PRESTO_SET_SCHEMA = "X-Presto-Set-Schema";
        public const string PRESTO_SET_PATH = "X-Presto-Set-Path";
        public const string PRESTO_SET_SESSION = "X-Presto-Set-Session";
        public const string PRESTO_CLEAR_SESSION = "X-Presto-Clear-Session";
        public const string PRESTO_SET_ROLE = "X-Presto-Set-Role";
        public const string PRESTO_ROLE = "X-Presto-Role";
        public const string PRESTO_PREPARED_STATEMENT = "X-Presto-Prepared-Statement";
        public const string PRESTO_ADDED_PREPARE = "X-Presto-Added-Prepare";
        public const string PRESTO_DEALLOCATED_PREPARE = "X-Presto-Deallocated-Prepare";
        public const string PRESTO_TRANSACTION_ID = "X-Presto-Transaction-Id";
        public const string PRESTO_STARTED_TRANSACTION_ID = "X-Presto-Started-Transaction-Id";
        public const string PRESTO_CLEAR_TRANSACTION_ID = "X-Presto-Clear-Transaction-Id";
        public const string PRESTO_CLIENT_INFO = "X-Presto-Client-Info";
        public const string PRESTO_CLIENT_TAGS = "X-Presto-Client-Tags";
        public const string PRESTO_CLIENT_CAPABILITIES = "X-Presto-Client-Capabilities";
        public const string PRESTO_RESOURCE_ESTIMATE = "X-Presto-Resource-Estimate";
        public const string PRESTO_EXTRA_CREDENTIAL = "X-Presto-Extra-Credential";

        public const string PRESTO_CURRENT_STATE = "X-Presto-Current-State";
        public const string PRESTO_MAX_WAIT = "X-Presto-Max-Wait";
        public const string PRESTO_MAX_SIZE = "X-Presto-Max-Size";
        public const string PRESTO_TASK_INSTANCE_ID = "X-Presto-Task-Instance-Id";
        public const string PRESTO_PAGE_TOKEN = "X-Presto-Page-Sequence-Id";
        public const string PRESTO_PAGE_NEXT_TOKEN = "X-Presto-Page-End-Sequence-Id";
        public const string PRESTO_BUFFER_COMPLETE = "X-Presto-Buffer-Complete";
    }
}
