using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SilentHunter.FileFormats.Dat;

public partial class DatFile
{
    [StructLayout(LayoutKind.Sequential)]
    private class Header
    {
        [Flags]
        public enum Flags : uint
        {
            /// <summary>
            /// This is the generic flag. Every file has this...
            /// </summary>
            Generic = 2,

            /// <summary>
            /// This flag is set when the file contains renderable objects.
            /// </summary>
            HasRenderableObjects = 0x10000,

            /// <summary>
            /// This flag is set when the file contains animations.
            /// </summary>
            HasAnimations = 0x1000000
        }

        /// <summary>
        /// Always 0x716d0da4
        /// </summary>
        // ReSharper disable once MemberHidesStaticFromOuterClass
        private readonly uint Magic;

        /// <summary>
        /// 0x00010002 and 0x00000002
        /// </summary>
        public Flags FileType;

        /// <summary>
        /// Always 0?
        /// </summary>
        private readonly uint Unknown1;

        public Header()
        {
            Magic = DatFile.Magic;
            FileType = Flags.Generic;
            Unknown1 = 0;
        }

        public bool IsValid()
        {
            if ((FileType & Flags.Generic) == 0)
            {
                Debug.WriteLine("Unexpected header flag");
            }

            return Magic == DatFile.Magic && Unknown1 == 0;
        }
    }
}