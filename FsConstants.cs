using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fat_file_system_cs
{
    
    internal static class FsConstants
    {
        // Size of one cluster in bytes
        public const int CLUSTER_SIZE = 1024;

        // Total number of clusters on the virtual disk
        public const int CLUSTER_COUNT = 1024;

        // Index of the superblock (cluster 0)
        public const int SUPERBLOCK_CLUSTER = 0;

        // FAT (File Allocation Table) region layout
        public const int FAT_START_CLUSTER = 1;           
        public const int FAT_END_CLUSTER = 9;             

        // Where file content starts (after FAT)
        public const int CONTENT_START_CLUSTER = 10;

        // First cluster of the root directory
        public const int ROOT_DIR_FIRST_CLUSTER = 10;     
    }
}
