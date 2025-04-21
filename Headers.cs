using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static SC2_3DS.Helper;

namespace SC2_3DS
{
    internal class Headers
    {
        public struct VMXHeader
        {
            public Byte4Array Magic;
            public ConsoleVersion ConsoleVersion;
            public byte Ukn01;
            public byte Ukn02;
            public byte Ukn03;
            public byte Ukn04;
            public ModelContent Contents;
            public ushort MatrixCount;
            public ushort Object0Count;
            public ushort Object1Count;
            public ushort Object2Count;
            public ushort BoneCount;
            public ushort MaterialCount;
            public ushort MeshCount;
            public uint TextureTableOffset;
            public uint MaterialOffset;
            public uint TextureMapOffset;
            public uint MatrixTableOffset;
            public uint MatrixUnkTableOffset;
            public uint Object0Offset;
            public uint Object1Offset;
            public uint Object2Offset;
            public uint WeightTableOffset;
            public uint Ukn01_offset;
            public uint BoneOffset;
            public uint BoneNameOffset;
            public uint Ukn02_offset;
        }
        //Xbox
        public static VMXHeader ReadVMXHeader(BinaryReader reader)
        {
            VMXHeader value = new VMXHeader
            {
                Magic = ReadByte4Array(reader),
                ConsoleVersion = (ConsoleVersion)reader.ReadByte(),
                Ukn01 = reader.ReadByte(),
                Ukn02 = reader.ReadByte(),
                Ukn03 = reader.ReadByte(),
                Ukn04 = reader.ReadByte(),
                Contents = (ModelContent)reader.ReadByte(),
                MatrixCount = ReadUInt16L(reader),
                Object0Count = ReadUInt16L(reader),
                Object1Count = ReadUInt16L(reader),
                Object2Count = ReadUInt16L(reader),
                BoneCount = ReadUInt16L(reader),
                MaterialCount = ReadUInt16L(reader),
                MeshCount = ReadUInt16L(reader),
                TextureTableOffset = ReadUInt32L(reader),
                MaterialOffset = ReadUInt32L(reader),
                TextureMapOffset = ReadUInt32L(reader),
                MatrixTableOffset = ReadUInt32L(reader),
                MatrixUnkTableOffset = ReadUInt32L(reader),
                Object0Offset = ReadUInt32L(reader),
                Object1Offset = ReadUInt32L(reader),
                Object2Offset = ReadUInt32L(reader),
                WeightTableOffset = ReadUInt32L(reader),
                Ukn01_offset = ReadUInt32L(reader),
                BoneOffset = ReadUInt32L(reader),
                BoneNameOffset = ReadUInt32L(reader),
                Ukn02_offset = ReadUInt32L(reader)
            };
            return value;
        }
        static ushort ShortNo = 0x4E4F;
        static uint DeadOffset = 0xDEADBEAF;
        public static void WriteVMXHeader(BinaryWriter f,VMXHeader hdr)
        {
            f.Write(hdr.Magic.ToBytes());
            f.Write((byte)hdr.ConsoleVersion);
            f.Write([hdr.Ukn01,hdr.Ukn02,hdr.Ukn03,hdr.Ukn04]);
            f.Write((byte)hdr.Contents);

            f.Write(hdr.MatrixCount);//MatrixCount
            f.Write(hdr.Object0Count);//Object0Count
            f.Write(hdr.Object1Count);//Object1Count
            f.Write(hdr.Object2Count);//Object2Count
            f.Write(hdr.BoneCount);//BoneCount
            f.Write(hdr.MaterialCount);//MaterialCount
            f.Write(hdr.MeshCount);//MeshCount

            f.Write(hdr.TextureTableOffset);//TextureTableOffset
            f.Write(hdr.MaterialOffset);//MaterialOffset
            f.Write(hdr.TextureMapOffset);//TextureMapOffset
            f.Write(hdr.MatrixTableOffset);//MatrixTableOffset
            f.Write(hdr.MatrixUnkTableOffset);//MatrixUnkTableOffset
            f.Write(hdr.Object0Offset);//Object0Offset
            f.Write(hdr.Object1Offset);//Object1Offset
            f.Write(hdr.Object2Offset);//Object2Offset
            f.Write(hdr.WeightTableOffset);//WeightTableOffset
            f.Write(hdr.Ukn01_offset);//Ukn01_offset
            f.Write(hdr.BoneOffset);//BoneOffset
            f.Write(hdr.BoneNameOffset);//BoneNameOffset
            f.Write(hdr.Ukn02_offset);//Ukn02_offset
        }

        //Gamecube
        public static VMXHeader ReadVMGHeader(BinaryReader reader)
        {
            VMXHeader value = new VMXHeader
            {
                Magic = ReadByte4Array(reader),
                ConsoleVersion = (ConsoleVersion)reader.ReadByte(),
                Ukn01 = reader.ReadByte(),
                Ukn02 = reader.ReadByte(),
                Ukn03 = reader.ReadByte(),
                Ukn04 = reader.ReadByte(),
                Contents = (ModelContent)reader.ReadByte(),
                MatrixCount = ReadUInt16B(reader),
                Object0Count = ReadUInt16B(reader),
                Object1Count = ReadUInt16B(reader),
                Object2Count = ReadUInt16B(reader),
                BoneCount = ReadUInt16B(reader),
                MaterialCount = ReadUInt16B(reader),
                MeshCount = ReadUInt16B(reader),
                TextureTableOffset = ReadUInt32B(reader),
                MaterialOffset = ReadUInt32B(reader),
                TextureMapOffset = ReadUInt32B(reader),
                MatrixTableOffset = ReadUInt32B(reader),
                MatrixUnkTableOffset = ReadUInt32B(reader),
                Object0Offset = ReadUInt32B(reader),
                Object1Offset = ReadUInt32B(reader),
                Object2Offset = ReadUInt32B(reader),
                WeightTableOffset = ReadUInt32B(reader),
                Ukn01_offset = ReadUInt32B(reader),
                BoneOffset = ReadUInt32B(reader),
                BoneNameOffset = ReadUInt32B(reader),
                Ukn02_offset = ReadUInt32B(reader)
            };
            return value;
        }

        public struct VXTHeader
        {
            public Byte4Array Magic; // VXT, VGT
            public byte Type; // Usually 0x01, 0x02 when clipping exists
            public byte Unk01_flag; // Usually 4
            public byte Unk02_flag; // Usually 2
            public byte Pad;
            public uint TextureCount;
            public uint Pad2; //Gamecube pad?
            public uint HeaderLength; // Usually 0014
            public uint HeaderBlockSize;
        }
        public static VXTHeader ReadVXTHeader(BinaryReader reader)
        {
            VXTHeader value = new VXTHeader
            {
                Magic = ReadByte4Array(reader),
                Type = reader.ReadByte(),
                Unk01_flag = reader.ReadByte(),
                Unk02_flag = reader.ReadByte(),
                Pad = reader.ReadByte(),
                TextureCount = ReadUInt32L(reader),
                HeaderLength = ReadUInt32L(reader),
                HeaderBlockSize = ReadUInt32L(reader),
            };
            return value;
        }

        public static VXTHeader ReadVGTHeader(BinaryReader reader)
        {
            VXTHeader value = new VXTHeader
            {
                Magic = ReadByte4Array(reader),
                Type = reader.ReadByte(),
                Unk01_flag = reader.ReadByte(),
                Unk02_flag = reader.ReadByte(),
                Pad = reader.ReadByte(),
                TextureCount = ReadUInt32B(reader),
                Pad2 = ReadUInt32B(reader),
                HeaderLength = ReadUInt32B(reader),
                HeaderBlockSize = ReadUInt32B(reader),
            };
            return value;
        }
    }
}
