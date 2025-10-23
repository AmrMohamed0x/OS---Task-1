using System;
using System.IO;
using System.Text;

namespace fat_file_system_cs
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Path where the virtual disk file will be stored (1 MB binary file)
            string diskPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\virtual_disk.bin");

            VirtualDisk vd = new VirtualDisk();

            try
            {
                // 1️⃣ Initialize (create if missing)
                vd.Initialize(diskPath, createIfMissing: true);
                Console.WriteLine("Virtual disk initialized successfully!");
                Console.WriteLine("Disk size: " + vd.GetDiskSize() + " bytes\n");

                // 2️⃣ Prepare a 1KB buffer to write to cluster 0
                byte[] writeBuffer = new byte[1024];
                string message = "Hello, Virtual Disk! This is a test message.";
                Encoding.ASCII.GetBytes(message, 0, message.Length, writeBuffer, 0);

                // 3️⃣ Write to cluster 0
                vd.WriteCluster(0, writeBuffer);
                Console.WriteLine("Data written to cluster 0.");

                // 4️⃣ Read cluster 0 back
                byte[] readBuffer = vd.ReadCluster(0);
                string result = Encoding.ASCII.GetString(readBuffer).TrimEnd('\0');
                Console.WriteLine("Read from cluster 0: " + result);

                // 5️⃣ Close the virtual disk
                vd.CloseDisk();
                Console.WriteLine("\nVirtual disk closed successfully.");
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
