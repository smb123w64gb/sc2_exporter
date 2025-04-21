
#if false
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using static SC2_3DS.Headers;
using static SC2_3DS.Helper;
using static SC2_3DS.Matrix;
using static SC2_3DS.Objects;
using static SC2_3DS.Textures;
using static SC2_3DS.Weight;

namespace SC2_3DS
{
    internal class ExportSC2 //Now reverse!
    {
        public static void WriteVMXObject(BinaryWriter writer, VMXObject input)
        {
            WriteVMXHeader(writer,input.VMXheader);
            //VMX HEADER
            input.Seek(0L, SeekOrigin.Begin);
            vmxobject.VMXheader = ReadVMXHeader(reader);
            //WEIGHTING
            input.Seek(vmxobject.VMXheader.WeightTableOffset, SeekOrigin.Begin);
            uint weightOffsetCheck = ReadUInt32L(reader);
            if (weightOffsetCheck != 0)
            {
                input.Seek(vmxobject.VMXheader.WeightTableOffset, SeekOrigin.Begin);
                vmxobject.WeightTables = ReadWeightTableXbox(reader);
                vmxobject.WeightDef1Bone = new WeightDefXbox[vmxobject.WeightTables.VertCount1 * 1];
                vmxobject.WeightDef2Bone = new WeightDefXbox[vmxobject.WeightTables.VertCount2 * 2];
                vmxobject.WeightDef3Bone = new WeightDefXbox[vmxobject.WeightTables.VertCount3 * 3];
                vmxobject.WeightDef4Bone = new List<WeightDefXbox>[vmxobject.WeightTables.VertCount4];
                input.Seek(vmxobject.WeightTables.WeightBufferOffset, SeekOrigin.Begin);
                for (uint i = 0; i < vmxobject.WeightTables.VertCount1; i++)
                    vmxobject.WeightDef1Bone[i] = ReadWeightDefXbox(reader);
                for (uint i = 0; i < vmxobject.WeightTables.VertCount2 * 2; i++)
                    vmxobject.WeightDef2Bone[i] = ReadWeightDefXbox(reader);
                for (uint i = 0; i < vmxobject.WeightTables.VertCount3 * 3; i++)
                    vmxobject.WeightDef3Bone[i] = ReadWeightDefXbox(reader);
                uint high = 4;
                for (uint i = 0; i < vmxobject.WeightTables.VertCount4; i++) { 
                    List < WeightDefXbox > CurrentItter = new List<WeightDefXbox>();
                    for (uint j = 0; j < high; j++) {
                        WeightDefXbox cur = ReadWeightDefXbox(reader);
                        CurrentItter.Add(new WeightDefXbox() { BoneIdx = cur.BoneIdx, BoneWeight = cur.BoneWeight, PositonXYZ = cur.PositonXYZ, NormalXYZ = cur.NormalXYZ,Stat=cur.Stat,Unk2=cur.Unk2,Unk3=cur.Unk3});
                        if(cur.Stat == 1){
                            high++;
                        }
                    }
                    vmxobject.WeightDef4Bone[i] = CurrentItter;
                }
            }
            //MATRIX Unknown
            input.Seek(vmxobject.VMXheader.MatrixUnkTableOffset, SeekOrigin.Begin);
            vmxobject.MatrixUnk = ReadMatrixUnkXbox(reader);
            //MATRIX
            vmxobject.MatrixTables = new MatrixTable[vmxobject.VMXheader.MatrixCount];
            vmxobject.MatrixDictionary = new Dictionary<int, int>();
            input.Seek(vmxobject.VMXheader.MatrixTableOffset, SeekOrigin.Begin);
            for (int i = 0; i < vmxobject.VMXheader.MatrixCount; i++)
            {
                vmxobject.MatrixTables[i] = ReadMatrixTableXbox(reader);
                vmxobject.MatrixDictionary.Add((int)vmxobject.VMXheader.MatrixTableOffset + (i * 400), i);
            }
            //VXT HEADER
            input.Seek(vmxobject.VMXheader.TextureTableOffset, SeekOrigin.Begin);
            vmxobject.VTXHeader = ReadVXTHeader(reader);
            //MATERIALS
            vmxobject.MaterialOffsets = new int[vmxobject.VMXheader.MaterialCount];
            vmxobject.MaterialTables = new MaterialTable[vmxobject.VMXheader.MaterialCount];
            vmxobject.MaterialDictionary = new Dictionary<int, int>();
            input.Seek(vmxobject.VMXheader.MaterialOffset, SeekOrigin.Begin);
            for (int i = 0; i < vmxobject.VMXheader.MaterialCount; i++)
            {
                vmxobject.MaterialOffsets[i] = (int)vmxobject.VMXheader.MaterialOffset + (i * 80);
                vmxobject.MaterialTables[i] = ReadMaterialTableXbox(reader);
                vmxobject.MaterialDictionary.Add((int)vmxobject.VMXheader.MaterialOffset + (i * 80), i);
            }
            //TEXTURES
            vmxobject.TextureTables = new TextureDataTypeXbox[vmxobject.VTXHeader.TextureCount];
            vmxobject.TextureDictionary = new Dictionary<int, int>();
            uint TextureTableOffset = vmxobject.VMXheader.TextureTableOffset + vmxobject.VTXHeader.HeaderLength;
            int TempSize = 0;
            for (int i = 0; i < vmxobject.VTXHeader.TextureCount; i++)
            {
                if (vmxobject.VTXHeader.Type == 0)
                {
                    input.Seek((int)TextureTableOffset + (i * 32), SeekOrigin.Begin);
                    vmxobject.TextureTables[i] = ReadTextureDataType0Xbox(reader);
                    vmxobject.TextureDictionary.Add((int)TextureTableOffset + (i * 32), i);
                }
                else if (vmxobject.VTXHeader.Type == 2)
                {
                    input.Seek((int)TextureTableOffset + (i * 36), SeekOrigin.Begin);
                    vmxobject.TextureTables[i] = ReadTextureDataType2Xbox(reader);
                    vmxobject.TextureDictionary.Add((int)TextureTableOffset + (i * 36), i);
                }
                input.Seek(vmxobject.VMXheader.TextureTableOffset + vmxobject.TextureTables[i].TextureDataOffset, SeekOrigin.Begin);
                TempSize = vmxobject.TextureTables[i].Width * vmxobject.TextureTables[i].Height;
                switch (vmxobject.TextureTables[i].ImageType)
                {
                    case ImageTypeXBOX.ARGB: vmxobject.TextureTables[i].DiffuseBytes = ReadTextureData(reader, TempSize); break;
                    case ImageTypeXBOX.P8: vmxobject.TextureTables[i].DiffuseBytes = ReadTextureData(reader, TempSize); break;
                    case ImageTypeXBOX.DXT1: TempSize >>= 1; vmxobject.TextureTables[i].DiffuseBytes = ReadTextureData(reader, TempSize); break; //DXT1 textures are half size
                    case ImageTypeXBOX.DXT3: vmxobject.TextureTables[i].DiffuseBytes = ReadTextureData(reader, TempSize); break;
                    case ImageTypeXBOX.DXT5: vmxobject.TextureTables[i].DiffuseBytes = ReadTextureData(reader, TempSize); break;
                }
                vmxobject.TextureTables[i].TextureSize = TempSize;
                if (vmxobject.TextureTables[i].MipMapCount > 1)
                {
                    vmxobject.TextureTables[i].MipMapBytes = new TextureData[vmxobject.TextureTables[i].MipMapCount - 1];
                    for (int j = 1; j < vmxobject.TextureTables[i].MipMapCount; j++)
                    {
                        TempSize = (vmxobject.TextureTables[i].Width / (2 * j)) * (vmxobject.TextureTables[i].Height / (2 * j));
                        vmxobject.TextureTables[i].MipMapBytes[j - 1] = ReadTextureData(reader, TempSize);
                    }
                }
                if (vmxobject.TextureTables[i].ImageType == ImageTypeXBOX.P8) //Palette
                {
                    input.Seek(vmxobject.VMXheader.TextureTableOffset + vmxobject.TextureTables[i].TexturePaletteCLUTOffset, SeekOrigin.Begin);
                    vmxobject.TextureTables[i].Palette = ReadTexturePaletteXbox(reader);
                    input.Seek(vmxobject.VMXheader.TextureTableOffset + vmxobject.TextureTables[i].Palette.PaletteOffset, SeekOrigin.Begin);
                    byte[] palettebuffer = reader.ReadBytes(vmxobject.TextureTables[i].Palette.PaletteCount * sizeof(int));
                    vmxobject.TextureTables[i].Palette.PaletteData = new byte[vmxobject.TextureTables[i].Palette.PaletteCount * 4];
                    Buffer.BlockCopy(palettebuffer, 0, vmxobject.TextureTables[i].Palette.PaletteData, 0, palettebuffer.Length);
                }
            }
            //BONES
            vmxobject.BoneTables = new BoneTable[vmxobject.VMXheader.BoneCount];
            vmxobject.BoneDictionary = new Dictionary<int, int>();
            for (int i = 0; i < vmxobject.VMXheader.BoneCount; i++)
            {
                input.Seek(vmxobject.VMXheader.BoneOffset + (i * 64), SeekOrigin.Begin);
                vmxobject.BoneTables[i] = ReadBoneTableXbox(reader);
                if (vmxobject.BoneTables[i].BoneNameOffset != 0)
                {
                    if (!vmxobject.BoneDictionary.ContainsKey(vmxobject.BoneTables[i].BoneParentIdx))
                    {
                        vmxobject.BoneDictionary.Add(vmxobject.BoneTables[i].BoneParentIdx, i);
                    }
                    input.Seek(vmxobject.BoneTables[i].BoneNameOffset, SeekOrigin.Begin);
                    vmxobject.BoneTables[i].Name = ReadNullTerminatedString(reader);
                }
                if (vmxobject.BoneTables[i].BoneNameOffset == 0)
                {
                    if (!vmxobject.BoneDictionary.ContainsKey(vmxobject.BoneTables[i].BoneParentIdx))
                    {
                        vmxobject.BoneDictionary.Add(vmxobject.BoneTables[i].BoneParentIdx, i);
                    }
                    vmxobject.BoneTables[i].Name = $"Empty_{i}";
                }
            }

            //MESH
            vmxobject.Object_0 = new LayerObjectEntryXbox[vmxobject.VMXheader.Object0Count];
            vmxobject.Object_1 = new LayerObjectEntryXbox[vmxobject.VMXheader.Object1Count];
            vmxobject.Object_2 = new LayerObjectEntryXbox[vmxobject.VMXheader.Object2Count];
            vmxobject.SkinnedMeshList = new List<LayerObjectEntryXbox>();
            vmxobject.StaticMeshList = new List<LayerObjectEntryXbox>();
            Vector3 TempVertSkinned = new Vector3(0, int.MaxValue, int.MinValue); //Total Verts, Min, Max
            //int[] TempVertSkinned = new int[3];
            TempVertSkinned[1] = 9001; //Min
            TempVertSkinned[2] = 0; //Max
            bool skinned_bool = false;
            for (int i = 0; i < vmxobject.VMXheader.Object0Count; i++)
            {
                input.Seek(vmxobject.VMXheader.Object0Offset + (i * 40), SeekOrigin.Begin);
                vmxobject.Object_0[i] = ReadLayerObjectEntryXbox(reader);
                if (vmxobject.Object_0[i].ObjectType == MeshXboxContent.SKINNED)
                {
                    vmxobject.Object_0[i].SkinnedMesh = ObjectSkinnedXboxHelper(vmxobject.Object_0[i], input, reader);
                    TempVertSkinned = ReadVertXbox((int)vmxobject.Object_0[i].FaceCount, vmxobject.Object_0[i].SkinnedMesh.Faces, (int)TempVertSkinned.Y, (int)TempVertSkinned.Z);
                    if (skinned_bool == false)
                    {
                        vmxobject.SkinnedData = vmxobject.Object_0[i];
                        skinned_bool = true;
                    }
                    vmxobject.SkinnedMeshList.Add(vmxobject.Object_0[i]);
                }
                else if (vmxobject.Object_0[i].ObjectType == MeshXboxContent.STATIC)
                {
                    vmxobject.Object_0[i].StaticMesh = ObjectStaticXboxHelper(vmxobject.Object_0[i], input, reader);
                    vmxobject.StaticMeshList.Add(vmxobject.Object_0[i]);
                }
            }
            for (int i = 0; i < vmxobject.VMXheader.Object1Count; i++)
            {
                input.Seek(vmxobject.VMXheader.Object1Offset + (i * 40), SeekOrigin.Begin);
                vmxobject.Object_1[i] = ReadLayerObjectEntryXbox(reader);
                if (vmxobject.Object_1[i].ObjectType == MeshXboxContent.SKINNED)
                {
                    vmxobject.Object_1[i].SkinnedMesh = ObjectSkinnedXboxHelper(vmxobject.Object_1[i], input, reader);
                    TempVertSkinned = ReadVertXbox((int)vmxobject.Object_1[i].FaceCount, vmxobject.Object_1[i].SkinnedMesh.Faces, (int)TempVertSkinned.Y, (int)TempVertSkinned.Z);
                    if (skinned_bool == false)
                    {
                        vmxobject.SkinnedData = vmxobject.Object_1[i];
                        skinned_bool = true;
                    }
                    vmxobject.SkinnedMeshList.Add(vmxobject.Object_1[i]);
                }
                else if (vmxobject.Object_1[i].ObjectType == MeshXboxContent.STATIC)
                {
                    vmxobject.Object_1[i].StaticMesh = ObjectStaticXboxHelper(vmxobject.Object_1[i], input, reader);
                    vmxobject.StaticMeshList.Add(vmxobject.Object_1[i]);
                }
            }
            for (int i = 0; i < vmxobject.VMXheader.Object2Count; i++)
            {
                input.Seek(vmxobject.VMXheader.Object2Offset + (i * 40), SeekOrigin.Begin);
                vmxobject.Object_2[i] = ReadLayerObjectEntryXbox(reader);
                if (vmxobject.Object_2[i].ObjectType == MeshXboxContent.SKINNED)
                {
                    vmxobject.Object_2[i].SkinnedMesh = ObjectSkinnedXboxHelper(vmxobject.Object_2[i], input, reader);
                    TempVertSkinned = ReadVertXbox((int)vmxobject.Object_2[i].FaceCount, vmxobject.Object_2[i].SkinnedMesh.Faces, (int)TempVertSkinned.Y, (int)TempVertSkinned.Z);
                    if (skinned_bool == false)
                    {
                        vmxobject.SkinnedData = vmxobject.Object_2[i];
                        skinned_bool = true;
                    }
                    vmxobject.SkinnedMeshList.Add(vmxobject.Object_2[i]);
                }
                else if (vmxobject.Object_2[i].ObjectType == MeshXboxContent.STATIC)
                {
                    vmxobject.Object_2[i].StaticMesh = ObjectStaticXboxHelper(vmxobject.Object_2[i], input, reader);
                    vmxobject.StaticMeshList.Add(vmxobject.Object_2[i]);
                }
            }
            //Skinned buffers are set here for convience
            if (skinned_bool) //There is a skinned mesh
            {
                vmxobject.Buffer1 = new Buffer1Xbox[(int)TempVertSkinned.X];
                vmxobject.Buffer2 = new Buffer2Xbox[(int)TempVertSkinned.X];
                vmxobject.Buffer3 = new Buffer2Xbox[(int)TempVertSkinned.X];
                input.Seek(vmxobject.SkinnedData.Buffer1Offset, SeekOrigin.Begin);
                for (int i = 0; i < TempVertSkinned[0]; i++)
                    vmxobject.Buffer1[i] = ReadBuffer1Xbox(reader);
                input.Seek(vmxobject.SkinnedData.Buffer2Offset, SeekOrigin.Begin);
                for (int i = 0; i < TempVertSkinned[0]; i++)
                    vmxobject.Buffer2[i] = ReadBuffer2Xbox(reader);
                input.Seek(vmxobject.SkinnedData.Buffer3Offset, SeekOrigin.Begin);
                for (int i = 0; i < TempVertSkinned[0]; i++)
                    vmxobject.Buffer3[i] = ReadBuffer2Xbox(reader);
            }
            return vmxobject;
        }
    }
}
#endif