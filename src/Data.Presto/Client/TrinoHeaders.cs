using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Client
{
    internal static class TrinoHeaders
    {
        public const string TRINO_USER = "X-Trino-User";
        public const string TRINO_SOURCE = "X-Trino-Source";
        public const string TRINO_CATALOG = "X-Trino-Catalog";
        public const string TRINO_SCHEMA = "X-Trino-Schema";
        public const string TRINO_PATH = "X-Trino-Path";
        public const string TRINO_TIME_ZONE = "X-Trino-Time-Zone";
        public const string TRINO_LANGUAGE = "X-Trino-Language";
        public const string TRINO_TRACE_TOKEN = "X-Trino-Trace-Token";
        public const string TRINO_SESSION = "X-Trino-Session";
        public const string TRINO_SET_CATALOG = "X-Trino-Set-Catalog";
        public const string TRINO_SET_SCHEMA = "X-Trino-Set-Schema";
        public const string TRINO_SET_PATH = "X-Trino-Set-Path";
        public const string TRINO_SET_SESSION = "X-Trino-Set-Session";
        public const string TRINO_CLEAR_SESSION = "X-Trino-Clear-Session";
        public const string TRINO_SET_ROLE = "X-Trino-Set-Role";
        public const string TRINO_ROLE = "X-Trino-Role";
        public const string TRINO_PREPARED_STATEMENT = "X-Trino-Prepared-Statement";
        public const string TRINO_ADDED_PREPARE = "X-Trino-Added-Prepare";
        public const string TRINO_DEALLOCATED_PREPARE = "X-Trino-Deallocated-Prepare";
        public const string TRINO_TRANSACTION_ID = "X-Trino-Transaction-Id";
        public const string TRINO_STARTED_TRANSACTION_ID = "X-Trino-Started-Transaction-Id";
        public const string TRINO_CLEAR_TRANSACTION_ID = "X-Trino-Clear-Transaction-Id";
        public const string TRINO_CLIENT_INFO = "X-Trino-Client-Info";
        public const string TRINO_CLIENT_TAGS = "X-Trino-Client-Tags";
        public const string TRINO_CLIENT_CAPABILITIES = "X-Trino-Client-Capabilities";
        public const string TRINO_RESOURCE_ESTIMATE = "X-Trino-Resource-Estimate";
        public const string TRINO_EXTRA_CREDENTIAL = "X-Trino-Extra-Credential";

        public const string TRINO_CURRENT_STATE = "X-Trino-Current-State";
        public const string TRINO_MAX_WAIT = "X-Trino-Max-Wait";
        public const string TRINO_MAX_SIZE = "X-Trino-Max-Size";
        public const string TRINO_TASK_INSTANCE_ID = "X-Trino-Task-Instance-Id";
        public const string TRINO_PAGE_TOKEN = "X-Trino-Page-Sequence-Id";
        public const string TRINO_PAGE_NEXT_TOKEN = "X-Trino-Page-End-Sequence-Id";
        public const string TRINO_BUFFER_COMPLETE = "X-Trino-Buffer-Complete";
    }
}
