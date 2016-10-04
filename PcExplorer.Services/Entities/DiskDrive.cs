using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcExplorer.Services.Entities
{
    public class DiskDrive
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string FileSystem { get; set; }
        public long AvailableSpace { get; set; }
        public long TotalSize { get; set; }
    }
}
