using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace fat_file_system_cs
{
    /// <summary>
    /// Handles reading and writing the Superblock (cluster 0).
    /// The Superblock stores general info about the filesystem.
    /// </summary>
    internal class SuperblockManager
    {
        private VirtualDisk disk;

        /// <summary>
        /// Constructor.
        /// Ensures the Superblock (cluster 0) starts as empty (zeros).
        /// </summary>
        public SuperblockManager(VirtualDisk disk)
        {
            this.disk = disk ?? throw new ArgumentNullException("disk");

            // Ensure the first cluster (superblock) is empty
            byte[] zeroData = new byte[FsConstants.CLUSTER_SIZE];
            disk.WriteCluster(FsConstants.SUPERBLOCK_CLUSTER, zeroData);
        }

        /// <summary>
        /// Reads the 1024-byte superblock (cluster 0).
        /// </summary>
        public byte[] ReadSuperblock()
        {
            return disk.ReadCluster(FsConstants.SUPERBLOCK_CLUSTER);
        }

        /// <summary>
        /// Writes data (exactly 1024 bytes) to the superblock.
        /// </summary>
        public void WriteSuperblock(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Length != FsConstants.CLUSTER_SIZE)
                throw new ArgumentException("Superblock must be exactly 1024 bytes.");

            disk.WriteCluster(FsConstants.SUPERBLOCK_CLUSTER, data);
        }
    }
}

