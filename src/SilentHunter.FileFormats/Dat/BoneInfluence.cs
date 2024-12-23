using System.Diagnostics;

namespace SilentHunter.FileFormats.Dat;

[DebuggerDisplay("1: {BoneIndex1} {Weight1} - 2: {BoneIndex2} {Weight2} - 3: {BoneIndex3} {Weight3} - 4: {BoneIndex4} {Weight4}\r\n")]
public struct BoneInfluence
{
    public int BoneIndex1;
    public float Weight1;
    public int BoneIndex2;
    public float Weight2;
    public int BoneIndex3;
    public float Weight3;
    public int BoneIndex4;
    public float Weight4;
}
