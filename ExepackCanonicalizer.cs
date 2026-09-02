namespace ElviraVgaEditor;

/// <summary>Shared, hash-gated EXEPACK RB canonicalizer. It has no file side effects.</summary>
internal static class ExepackCanonicalizer
{
    internal static byte[] Unpack(byte[] packed, int expectedSize, string expectedHash, string name, Action<byte[]>? finalize = null)
    {
        if (packed.Length != expectedSize || Hash(packed) != expectedHash) throw new InvalidDataException($"The packed {name} fingerprint is unsupported.");
        if (packed.Length < 28 || packed[0] != 'M' || packed[1] != 'Z') throw new InvalidDataException($"The packed {name} is not MZ.");
        int hp=U16(packed,8), cs=U16(packed,0x16), xp=(hp+cs)*16;
        if (xp<hp*16 || xp+18>packed.Length) throw new InvalidDataException($"{name} EXEPACK header is outside the image.");
        int len=U16(packed,xp+12)*16; if (U16(packed,xp+14)!=0x4252 && U16(packed,xp+16)!=0x4252) throw new InvalidDataException($"{name} EXEPACK RB signature is absent.");
        byte[] stream=packed[(hp*16)..xp].Reverse().ToArray(), module=new byte[len]; int s=0,d=0; while(s<stream.Length&&stream[s]==0xFF)s++; bool stop=false;
        while(!stop&&s<stream.Length){if(s+3>stream.Length)throw new InvalidDataException("Truncated EXEPACK command.");byte op=stream[s++];int count=(stream[s++]<<8)|stream[s++];if(d+count>module.Length)throw new InvalidDataException("EXEPACK command exceeds output.");if((op&0xFE)==0xB0){if(s>=stream.Length)throw new InvalidDataException("Truncated EXEPACK fill.");Array.Fill(module,stream[s++],d,count);}else if((op&0xFE)==0xB2){if(s+count>stream.Length)throw new InvalidDataException("Truncated EXEPACK copy.");Array.Copy(stream,s,module,d,count);s+=count;}else throw new InvalidDataException($"Unsupported EXEPACK opcode 0x{op:X2}.");d+=count;stop=(op&1)!=0;}
        int remain=stream.Length-s;if(remain>module.Length-d)throw new InvalidDataException("EXEPACK tail exceeds output.");Array.Copy(stream,s,module,d,remain);int moduleLen=d+remain;Array.Reverse(module,0,moduleLen);
        int mark=IndexOf(packed,"Packed file is corrupt"u8.ToArray(),xp);if(mark<0)throw new InvalidDataException("EXEPACK relocation marker missing.");int rs=mark+22;var rel=new List<(ushort O,ushort S)>();for(int seg=0;seg<16;seg++){ushort n=U16(packed,rs);rs+=2;for(int i=0;i<n;i++){rel.Add((U16(packed,rs),(ushort)(seg*0x1000)));rs+=2;}}
        const int mz=28;int paras=((mz+rel.Count*4)/16/32+1)*32, header=paras*16;var result=new byte[header+moduleLen];W16(result,0,0x5A4D);W16(result,2,(ushort)(result.Length%512));W16(result,4,(ushort)((result.Length+511)/512));W16(result,6,(ushort)rel.Count);W16(result,8,(ushort)paras);W16(result,0xA,U16(packed,0xA));W16(result,0xC,0xFFFF);W16(result,0xE,U16(packed,xp+10));W16(result,0x10,U16(packed,xp+8));W16(result,0x14,U16(packed,xp));W16(result,0x16,U16(packed,xp+2));W16(result,0x18,mz);int at=mz;foreach(var e in rel){W16(result,at,e.O);W16(result,at+2,e.S);at+=4;}Array.Copy(module,0,result,header,moduleLen);finalize?.Invoke(result);return result;
    }
    private static ushort U16(byte[] b,int o)=>(ushort)(b[o]|b[o+1]<<8);private static void W16(byte[] b,int o,ushort v){b[o]=(byte)v;b[o+1]=(byte)(v>>8);}private static string Hash(byte[] b)=>Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(b));private static int IndexOf(byte[] a,byte[] n,int start){for(int i=start;i<=a.Length-n.Length;i++)if(a.AsSpan(i,n.Length).SequenceEqual(n))return i;return -1;}
}
