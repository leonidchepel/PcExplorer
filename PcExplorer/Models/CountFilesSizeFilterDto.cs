using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PcExplorer.Models
{
    public class CountFilesSizeFilterDto
    {
        public long FromBytes { get; set; }
        public long ToBytes { get; set; }
        
    }
}