using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryJournal.Net
{
    public class binaryNodeList
    {
        MemoryStream? ms = null;
        BinaryReader? br = null;
        BinaryWriter? bw = null;

        public binaryNodeList()
        {
            this.ms = new MemoryStream();
            this.br = new BinaryReader(ms);
            this.bw = new BinaryWriter(ms);
        }

        public void writeNode(UInt32 nodeID, UInt32 parentNodeID)
        {
            byte[] row = new byte[sizeof(UInt32) + sizeof(UInt32)];
            byte[] nodeIDBytes  = BitConverter.GetBytes(nodeID);
            byte[] parentNodeIDBytes = BitConverter.GetBytes(parentNodeID);
            Array.Copy(nodeIDBytes, 0, row, 0, sizeof(UInt32));
            Array.Copy(parentNodeIDBytes, 0, row, sizeof(UInt32), sizeof(UInt32));
            bw.Seek(0, SeekOrigin.End);
            bw.Write(row, 0, row.Length);
            bw.Flush();
        }
        public void readNode(ref UInt32 nodeID, ref UInt32 parentNodeID)
        {
            byte[] row = new byte[sizeof(UInt32) + sizeof(UInt32)];
            br.Read(row, 0, row.Length);
            nodeID = BitConverter.ToUInt32(row, 0);
            parentNodeID = BitConverter.ToUInt32(row, sizeof(UInt32));
        }

        public void setPosition(long pos)
        {
            ms.Position = pos;
        }

    }
}
