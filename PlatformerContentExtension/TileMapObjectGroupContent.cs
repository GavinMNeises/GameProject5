using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerContentExtension
{
    //Contains all of the information for an objectgroup object
    public class ObjectGroupObjects
    {
        public int SheetIndex { get; set; }
        
        public uint X { get; set; }

        public uint Y { get; set; }

        public uint Width { get; set; }

        public uint Height { get; set; }
    }

    //Contains all of the information of an objectgroup
    public class TilemapObjectGroupContent
    {
        public uint Id { get; set; }

        public string Name { get; set; }

        public uint Width { get; set; }

        public List<ObjectGroupObjects> Objects = new List<ObjectGroupObjects>();
    }
}
