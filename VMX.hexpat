enum  modelContent : u8{
    STAGE     = 0,
    CHARACTER = 1,
    WEAPON    = 2
};
struct matrixUnk{
u16 unk;
u16 Matrix2Count;
u32 Matrix2Offset;
};
struct tVector4f{
float x,y,z,w;
};
struct matrixTable{
u8 Type;
u8 ParentBoneIdx;
u16 unk0;
u32 stageunk;
u32 unk2;
u32 unk3;
tVector4f Matrix4x4Transform[4];
u8 pad[320];
};

struct VXT_Header{
char MAGIC[4];
u8 type;
u8 unk0;
u8 unk1;
u8 pad;
u32 TexRowCount;
u32 HeaderLength;
u32 HeaderBlockSize;
};
struct VMX_Header{
char MAGIC [4];
u8 version;
if(version == 3){
#pragma big
} else if(version == 4) {
#pragma little
}
u32 unk0;
modelContent Contents;

u16 MatricesCount;
u16 Objects0Count;
u16 Objects1Count;
u16 Objects2Count;
u16 BonesCount;
u16 MaterialsCount;
u16 MeshCount; // always 1?
u32 TextureTableOffset;
u32 MaterialOffset; 
u32 TextureMapOffset;
u32 MatrixTableOffset;
u32 ukn_MatrixTableOffset;
u32 Object0Offset;
u32 Object1Offset;
u32 Object2Offset;
u32 WeightTableOffset;
u32 ukn01_offset;
u32 BoneOffset;
u32 BoneNameOffset;
u32 BoneHeaderOffset;

};
struct VMX{
VMX_Header header;
matrixUnk unkmatrix @ header.ukn_MatrixTableOffset;
matrixTable matrix_table[header.MatricesCount] @ header.MatrixTableOffset; 
VXT_Header vxt_header @ header.TextureTableOffset;
};
VMX file @ 0;