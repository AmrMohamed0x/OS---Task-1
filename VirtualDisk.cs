using System;
using System.IO;

namespace fat_file_system_cs
{
    internal class VirtualDisk
    {
        private const int CLUSTER_SIZE = 1024;
        private const int CLUSTERS_NUMBER = 1024;

        private long diskSize = 0;
        private string diskPath = null;
        private FileStream diskFileStream = null;
        private bool isOpen = false;

        public void Initialize(string path, bool createIfMissing = true)
        {
            if (this.isOpen)
                throw new InvalidOperationException("Disk is already initialized.");

            this.diskPath = path;
            this.diskSize = CLUSTERS_NUMBER * CLUSTER_SIZE;

            try
            {
                if (!File.Exists(diskPath))
                {
                    if (createIfMissing)
                        this.CreateEmptyDisk(path);
                    else
                        throw new FileNotFoundException("Couldn't find the specified disk path");
                }

                this.diskFileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                this.isOpen = true;
            }
            catch (Exception ex)
            {
                this.isOpen = false;
                throw new IOException("Failed to open disk: " + ex.Message, ex);
            }
        }

        private void CreateEmptyDisk(string path)
        {
            try
            {
                using (FileStream tempFileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    byte[] emptyClusterPlaceholder = new byte[CLUSTER_SIZE];
                    for (int i = 0; i < CLUSTERS_NUMBER; i++)
                        tempFileStream.Write(emptyClusterPlaceholder, 0, CLUSTER_SIZE);

                    tempFileStream.Flush();
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to create disk file: " + ex.Message, ex);
            }
        }

        public byte[] ReadCluster(int clusterIndex)
        {
            EnsureOpen();

            if (clusterIndex < 0 || clusterIndex >= CLUSTERS_NUMBER)
                throw new ArgumentOutOfRangeException("clusterIndex",
                    "Cluster index must be between 0 and " + (CLUSTERS_NUMBER - 1) + ".");

            byte[] buffer = new byte[CLUSTER_SIZE];
            long offset = clusterIndex * CLUSTER_SIZE;

            lock (diskFileStream)
            {
                diskFileStream.Seek(offset, SeekOrigin.Begin);
                int bytesRead = diskFileStream.Read(buffer, 0, CLUSTER_SIZE);
                if (bytesRead < CLUSTER_SIZE)
                    throw new IOException("Failed to read full cluster (unexpected EOF).");
            }

            return buffer;
        }

        public void WriteCluster(int clusterIndex, byte[] data)
        {
            EnsureOpen();

            if (clusterIndex < 0 || clusterIndex >= CLUSTERS_NUMBER)
                throw new ArgumentOutOfRangeException("clusterIndex",
                    "Cluster index must be between 0 and " + (CLUSTERS_NUMBER - 1) + ".");

            if (data == null || data.Length != CLUSTER_SIZE)
                throw new ArgumentException("Data must be exactly " + CLUSTER_SIZE + " bytes.", "data");

            long offset = clusterIndex * CLUSTER_SIZE;

            lock (diskFileStream)
            {
                diskFileStream.Seek(offset, SeekOrigin.Begin);
                diskFileStream.Write(data, 0, CLUSTER_SIZE);
                diskFileStream.Flush();
            }
        }

        public long GetDiskSize()
        {
            EnsureOpen();
            return diskSize;
        }

        public void CloseDisk()
        {
            if (!isOpen)
                return;

            try
            {
                if (diskFileStream != null)
                {
                    diskFileStream.Flush();
                    diskFileStream.Close();
                    diskFileStream = null;
                }
            }
            finally
            {
                isOpen = false;
            }
        }

        private void EnsureOpen()
        {
            if (!isOpen || diskFileStream == null)
                throw new InvalidOperationException("Virtual disk is not initialized or already closed.");
        }
    }
}
