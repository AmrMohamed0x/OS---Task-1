using System;
using System.IO;
using System.Text;

namespace fat_file_system_cs
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string diskPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\virtual_disk.bin");
            VirtualDisk vd = new VirtualDisk();

            try
            {
                // Initialize virtual disk
                vd.Initialize(diskPath, true);
                Console.WriteLine("Disk initialized.");

                // Create Superblock manager
                SuperblockManager sbm = new SuperblockManager(vd);

                // Write some text into the superblock
                byte[] superData = new byte[FsConstants.CLUSTER_SIZE];
                string info = "Simple Superblock initialized successfully!";
                Encoding.ASCII.GetBytes(info, 0, info.Length, superData, 0);

                sbm.WriteSuperblock(superData);
                Console.WriteLine("Superblock written.");

                // Read it back
                byte[] readBack = sbm.ReadSuperblock();
                string text = Encoding.ASCII.GetString(readBack).TrimEnd('\0');
                Console.WriteLine("Superblock content: " + text);

                vd.CloseDisk();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
