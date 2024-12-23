using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using FluentAssertions;
using SilentHunter.Testing.FluentAssertions;
using Xunit;

namespace SilentHunter.FileFormats.Extensions;

public class BinaryReaderExtensionTests
{
    private const string Utf8String = "Şớოε şặмрĺê ÄŚĈÍ|-ť℮χŧ";
    private static readonly char[] NullTerminator = { '\0' };
    private static readonly char[] CrLfTerminators = { '\r', '\n' };
    private static readonly string CrLfTerminator = "\r\n";

    [Fact]
    public void it_will_read_color()
    {
        System.Drawing.Color expected = System.Drawing.Color.FromArgb(0x33, 0x66, 0x99, 0xcc);
        using (var reader = new BinaryReader(new MemoryStream(new[] { expected.A, expected.R, expected.G, expected.B })))
        {
            var color = reader.ReadStruct<System.Drawing.Color>();
            color.Should().BeOfType<System.Drawing.Color>().And.Be(expected);
            reader.BaseStream.Should().BeEof();
        }
    }

    private enum IntEnum
    {
        A = 100,
        B
    }

    [Fact]
    public void it_will_read_enum()
    {
        const IntEnum expected = IntEnum.B;
        using (var reader = new BinaryReader(new MemoryStream(BitConverter.GetBytes((int)expected))))
        {
            var value = reader.ReadStruct<IntEnum>();
            value.Should().Be(expected);
            reader.BaseStream.Should().BeEof();
        }
    }

    [Fact]
    public void it_will_read_nullable_enum()
    {
        const IntEnum expected = IntEnum.B;
        using (var reader = new BinaryReader(new MemoryStream(BitConverter.GetBytes((int)expected))))
        {
            var value = reader.ReadStruct<IntEnum?>();
            value.Should().Be(expected);
            reader.BaseStream.Should().BeEof();
        }
    }

    private enum LongEnum : ulong
    {
        A = 1234567890123456,
        B
    }

    [Fact]
    public void it_will_read_enum_using_alternate_underlying_type()
    {
        const LongEnum expected = LongEnum.B;
        using (var reader = new BinaryReader(new MemoryStream(BitConverter.GetBytes((long)expected))))
        {
            var value = reader.ReadStruct<LongEnum>();
            value.Should().Be(expected);
            reader.BaseStream.Should().BeEof();
        }
    }

    [Fact]
    public void it_will_read_bool_true()
    {
        using (var reader = new BinaryReader(new MemoryStream(new byte[] { 0xA0 })))
        {
            bool value = reader.ReadStruct<bool>();
            value.Should().BeTrue();
            reader.BaseStream.Should().BeEof();
        }
    }

    [Fact]
    public void it_will_read_bool_false()
    {
        using (var reader = new BinaryReader(new MemoryStream(new byte[] { 0 })))
        {
            bool value = reader.ReadStruct<bool>();
            value.Should().BeFalse();
            reader.BaseStream.Should().BeEof();
        }
    }

    [Fact]
    public void it_will_read_nullable_bool_true()
    {
        using (var reader = new BinaryReader(new MemoryStream(new byte[] { 0xA0 })))
        {
            var value = reader.ReadStruct<bool?>();
            value.Should().BeTrue();
            reader.BaseStream.Should().BeEof();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct CustomStruct
    {
        public byte Byte;
        public sbyte Sbyte;
        public short Short;
        public ushort UShort;
        public int Int32;
        public uint UInt32;
        public long Int64;
        public ulong UInt64;
        public bool Bool;
        public float Float;
        public double Double;
        public decimal Decimal;
    }

    [Fact]
    public void it_will_read_struct()
    {
        var expected = new CustomStruct
        {
            Byte = 0x10,
            Sbyte = -0x10,
            UShort = 0x2ABC,
            Short = -0x2ABC,
            UInt32 = 0x400ABC00,
            Int32 = -0x400ABC00,
            UInt64 = 0x7000000ABC000000,
            Int64 = -0x7000000ABC000000,
            Bool = true,
            Float = 12345.678f,
            Double = 123456789.123456789d,
            Decimal = 123456789.123456789m
        };

        //var size = Marshal.SizeOf(expected);
        //var ptr = Marshal.AllocHGlobal(size);
        //var expectedBytes = new byte[size];
        //Marshal.StructureToPtr(expected, ptr, false);
        //Marshal.Copy(ptr, expectedBytes, 0, size);

        //var test = Marshal.PtrToStructure(ptr, expected.GetType());

        //expectedBytes.ToList().ForEach(b => Debug.Write("0x" + b.ToString("x2") + ", "));
        var source = new byte[] { 0x10, 0xf0, 0x44, 0xd5, 0xbc, 0x2a, 0x00, 0x44, 0xf5, 0xbf, 0x00, 0xbc, 0x0a, 0x40, 0x00, 0x00, 0x00, 0x44, 0xf5, 0xff, 0xff, 0x8f, 0x00, 0x00, 0x00, 0xbc, 0x0a, 0x00, 0x00, 0x70, 0x01, 0x00, 0x00, 0x00, 0xb6, 0xe6, 0x40, 0x46, 0x75, 0x6b, 0x7e, 0x54, 0x34, 0x6f, 0x9d, 0x41, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x15, 0x5f, 0xd0, 0xac, 0x4b, 0x9b, 0xb6, 0x01 };

        using (var reader = new BinaryReader(new MemoryStream(source)))
        {
            var value = reader.ReadStruct<CustomStruct>();
            value.Should().Be(expected);
            reader.BaseStream.Should().BeEof();
        }

        //Marshal.FreeHGlobal(ptr);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
    private struct StringStructByval
    {
        public int First;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string Text;
        public int Last;
    }

    [Fact]
    public void it_will_read_struct_with_string_byval()
    {
        var expected = new StringStructByval
        {
            First = 0x12345678,
            Text = Utf8String,
            Last = 0x76543210
        };

        var source = new byte[] { 0x78, 0x56, 0x34, 0x12, 0x5e, 0x01, 0xdb, 0x1e, 0xdd, 0x10, 0xb5, 0x03, 0x20, 0x00, 0x5f, 0x01, 0xb7, 0x1e, 0x3c, 0x04, 0x40, 0x04, 0x3a, 0x01, 0xea, 0x00, 0x20, 0x00, 0xc4, 0x00, 0x5a, 0x01, 0x08, 0x01, 0xcd, 0x00, 0x7c, 0x00, 0x2d, 0x00, 0x65, 0x01, 0x2e, 0x21, 0xc7, 0x03, 0x67, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x32, 0x54, 0x76 };

        using (var reader = new BinaryReader(new MemoryStream(source)))
        {
            var value = reader.ReadStruct<StringStructByval>();
            value.Should().Be(expected);
            reader.BaseStream.Should().BeEof();
        }
    }

    [Fact]
    public void it_will_throw_when_type_size_exceeds_available_data()
    {
        using (var reader = new BinaryReader(new MemoryStream(new byte[] { 0, 1, 2, 3 })))
        {
            Action action = () => reader.ReadStruct<CustomStruct>();
            action.Should().Throw<EndOfStreamException>();
            reader.BaseStream.Should().BeEof();
        }
    }

    [Fact]
    public void it_will_read_string_of_arbitrary_size()
    {
        int count = 8;
        int count2 = Utf8String.Length - count;
        using (var reader = new BinaryReader(new MemoryStream(Encoding.UTF8.GetBytes(Utf8String))))
        {
            reader.ReadString(count).Should().Be(Utf8String.Substring(0, count));
            reader.ReadString(count2).Should().Be(Utf8String.Substring(count));
            reader.BaseStream.Should().BeEof();
        }
    }

    [Fact]
    public void it_will_throw_when_arbitrary_size_exceeds_available_data()
    {
        int count = 5000;
        using (var reader = new BinaryReader(new MemoryStream(Encoding.UTF8.GetBytes("Small string"))))
        {
            Action action = () => reader.ReadString(count);
            action.Should().Throw<EndOfStreamException>();
            reader.BaseStream.Should().BeEof();
        }
    }

    [Fact]
    public void it_will_read_strings_terminated_by_null()
    {
        using (var reader = new BinaryReader(new MemoryStream(Encoding.UTF8.GetBytes(string.Format("First\0\0{0}\0Last\0", Utf8String)))))
        {
            reader.ReadString(NullTerminator).Should().Be("First");
            reader.ReadString(NullTerminator).Should().BeEmpty();
            reader.ReadString(NullTerminator).Should().Be(Utf8String);
            reader.ReadString(NullTerminator).Should().Be("Last");
            reader.BaseStream.Should().BeEof();
        }
    }

    [Fact]
    public void it_will_read_strings_terminated_by_any_character()
    {
        using (var reader = new BinaryReader(new MemoryStream(Encoding.UTF8.GetBytes(string.Format("First\r\n\r\n{0}\r\nLast\r\n", Utf8String)))))
        {
            reader.ReadString(CrLfTerminators).Should().Be("First");
            reader.ReadString(CrLfTerminators).Should().BeEmpty();
            reader.ReadString(CrLfTerminators).Should().BeEmpty();
            reader.ReadString(CrLfTerminators).Should().BeEmpty();
            reader.ReadString(CrLfTerminators).Should().Be(Utf8String);
            reader.ReadString(CrLfTerminators).Should().BeEmpty();
            reader.ReadString(CrLfTerminators).Should().Be("Last");
            reader.ReadString(CrLfTerminators).Should().BeEmpty();
            reader.BaseStream.Should().BeEof();
        }
    }

    [Fact]
    public void it_will_read_strings_terminated_by_string()
    {
        using (var reader = new BinaryReader(new MemoryStream(Encoding.UTF8.GetBytes(string.Format("Fir\rst\r\n\r\n{0}\r\nL\rast\r\n", Utf8String)))))
        {
            string result = reader.ReadString(CrLfTerminator).Should().Be("Fir\rst").And.Subject;
            reader.ReadString(CrLfTerminator).Should().BeEmpty();
            reader.ReadString(CrLfTerminator).Should().Be(Utf8String);
            reader.ReadString(CrLfTerminator).Should().Be("L\rast");
            reader.BaseStream.Should().BeEof();
        }

        const string terminator = "123456789";

        using (var reader = new BinaryReader(new MemoryStream(Encoding.UTF8.GetBytes(string.Format("abc1234def{0}ghi{0}{0}jklmno1234{0}123pqrs{0}", terminator)))))
        {
            reader.ReadString(terminator).Should().Be("abc1234def");
            reader.ReadString(terminator).Should().Be("ghi");
            reader.ReadString(terminator).Should().BeEmpty();
            reader.ReadString(terminator).Should().Be("jklmno1234");
            reader.ReadString(terminator).Should().Be("123pqrs");
            reader.BaseStream.Should().BeEof();
        }
    }

    [Fact]
    public void it_will_throw_when_string_not_terminated()
    {
        using (var reader = new BinaryReader(new MemoryStream(Encoding.UTF8.GetBytes("String"))))
        {
            Action action = () => reader.ReadString(NullTerminator);
            action.Should().Throw<EndOfStreamException>();
            reader.BaseStream.Should().BeEof();
        }
    }
}