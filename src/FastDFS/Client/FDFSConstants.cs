namespace FastDFS.Client
{
    internal class FDFSConstants
    {
        public const byte FDFS_PROTO_CMD_QUIT = 82;
        public const byte TRACKER_PROTO_CMD_SERVER_LIST_GROUP = 91;
        public const byte TRACKER_PROTO_CMD_SERVER_LIST_STORAGE = 92;
        public const byte TRACKER_PROTO_CMD_SERVER_DELETE_STORAGE = 93;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITHOUT_GROUP_ONE = 101;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ONE = 102;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_UPDATE = 103;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE = 104;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ALL = 105;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITHOUT_GROUP_ALL = 106;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ALL = 107;
        public const byte TRACKER_PROTO_CMD_RESP = 100;
        public const byte FDFS_PROTO_CMD_ACTIVE_TEST = 111;
        public const byte STORAGE_PROTO_CMD_UPLOAD_FILE = 11;
        public const byte STORAGE_PROTO_CMD_DELETE_FILE = 12;
        public const byte STORAGE_PROTO_CMD_SET_METADATA = 13;
        public const byte STORAGE_PROTO_CMD_DOWNLOAD_FILE = 14;
        public const byte STORAGE_PROTO_CMD_GET_METADATA = 15;
        public const byte STORAGE_PROTO_CMD_UPLOAD_SLAVE_FILE = 21;
        public const byte STORAGE_PROTO_CMD_QUERY_FILE_INFO = 22;
        public const byte STORAGE_PROTO_CMD_UPLOAD_APPENDER_FILE = 23;  //create appender file
        public const byte STORAGE_PROTO_CMD_APPEND_FILE = 24;  //append file
        public const byte STORAGE_PROTO_CMD_MODIFY_FILE = 34;  //modify appender file
        public const byte STORAGE_PROTO_CMD_TRUNCATE_FILE = 36;  //truncate appender file
        public const byte STORAGE_PROTO_CMD_RESP = TRACKER_PROTO_CMD_RESP;

        public enum ErrorCode
        {
            ERR_NO_ENOENT = 2,
            ERR_NO_EIO = 5,
            ERR_NO_EBUSY = 16,
            ERR_NO_EINVAL = 22,
            ERR_NO_ENOSPC = 28,
            ECONNREFUSED = 61,
            ERR_NO_EALREADY = 114
        }
    }
}
