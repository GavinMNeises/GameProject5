using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformLibrary
{
    /// <summary>
    /// Represents an object of an objectgroup
    /// </summary>
    public class GroupObject
    {
        public int SheetIndex;

        public uint X;

        public uint Y;

        public uint Width;

        public uint Height;

        public GroupObject(int sheetIndex, uint x, uint y, uint width, uint height)
        {
            SheetIndex = sheetIndex;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
