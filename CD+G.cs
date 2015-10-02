//
//  CD+G.cs
//
//  Author:
//       Natalia Portillo <claunia@claunia.com>
//
//  Copyright (c) 2015 © Claunia.com
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using System;

namespace SubChannelDecoder
{
    public static class CD_G
    {
        public struct CD_RW_Packet
        {
            public byte command;
            public byte instruction;
            public byte[] parityQ;
            public byte[] data;
            public byte[] parityP;
        }

        public enum CD_G_Instructions : byte
        {
            CDG_Memory_Preset = 1,
            CDG_Border_Preset = 2,
            CDG_Tile_Block = 6,
            CDG_Scroll_Preset = 20,
            CDG_Scroll_Copy = 24,
            CDG_Define_Transparent = 28,
            CDG_Load_Color_Table_0_7 = 30,
            CDG_Load_Color_Table_8_15 = 31,
            CDG_Tile_Block_XOR = 38
        }

        public const byte CD_G_Command = 0x09;

        static readonly int[] offsets = new []
        { 0, 66, 125, 191, 100, 50, 150, 175,
            8, 33, 58, 83, 108, 133, 158, 183,
            16, 41, 25, 91, 116, 141, 166, 75
        };

        public static CD_RW_Packet[] Packetize_CD_RW(byte[] subchannels)
        {
            Console.WriteLine("{0}", subchannels.Length);
            CD_RW_Packet[] packets = new CD_RW_Packet[4];

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            for (int pack = 0; pack < 4; pack++)
                for (int column = 0; column < 24; column++)
                    ms.WriteByte(subchannels[(pack*24)+offsets[column]]);

            ms.Seek(0, System.IO.SeekOrigin.Begin);
            byte[] packetized = ms.ToArray();

            for (int i = 0; i < 4; i++)
            {
                packets[i].parityQ = new byte[2];
                packets[i].data = new byte[16];
                packets[i].parityP = new byte[4];

                packets[i].command = (byte)(packetized[0 + i * 24] & 0x3F);
                packets[i].instruction = (byte)(packetized[1 + i * 24] & 0x3F);

                packets[i].parityQ[0] = (byte)(packetized[2 + i * 24] & 0x3F);
                packets[i].parityQ[1] = (byte)(packetized[3 + i * 24] & 0x3F);
                packets[i].data[0] = (byte)(packetized[4 + i * 24] & 0x3F);
                packets[i].data[1] = (byte)(packetized[5 + i * 24] & 0x3F);
                packets[i].data[2] = (byte)(packetized[6 + i * 24] & 0x3F);
                packets[i].data[3] = (byte)(packetized[7 + i * 24] & 0x3F);
                packets[i].data[4] = (byte)(packetized[8 + i * 24] & 0x3F);
                packets[i].data[5] = (byte)(packetized[9 + i * 24] & 0x3F);
                packets[i].data[6] = (byte)(packetized[10 + i * 24] & 0x3F);
                packets[i].data[7] = (byte)(packetized[11 + i * 24] & 0x3F);
                packets[i].data[8] = (byte)(packetized[12 + i * 24] & 0x3F);
                packets[i].data[9] = (byte)(packetized[13 + i * 24] & 0x3F);
                packets[i].data[10] = (byte)(packetized[14 + i * 24] & 0x3F);
                packets[i].data[11] = (byte)(packetized[15 + i * 24] & 0x3F);
                packets[i].data[12] = (byte)(packetized[16 + i * 24] & 0x3F);
                packets[i].data[13] = (byte)(packetized[17 + i * 24] & 0x3F);
                packets[i].data[14] = (byte)(packetized[18 + i * 24] & 0x3F);
                packets[i].data[15] = (byte)(packetized[19 + i * 24] & 0x3F);
                packets[i].parityP[0] = (byte)(packetized[20 + i * 24] & 0x3F);
                packets[i].parityP[1] = (byte)(packetized[21 + i * 24] & 0x3F);
                packets[i].parityP[2] = (byte)(packetized[22 + i * 24] & 0x3F);
                packets[i].parityP[3] = (byte)(packetized[23 + i * 24] & 0x3F);
            }

            return packets;
        }

        public static void PrintCDGPackets(byte[] subchannel)
        {
            PrintCDGPackets(Packetize_CD_RW(subchannel));
        }

        public static void PrintCDGPackets(CD_RW_Packet[] packets)
        {
            for (int i = 0; i < packets.Length; i++)
            {
                Console.WriteLine("CD+G Packet {0}", i);
                PrintCDGPacket(packets[i]);
            }
        }

        public static void PrintCDGPacket(CD_RW_Packet packet)
        {
            if (packet.command != CD_G_Command)
                return;

            switch (packet.instruction)
            {
                case (byte)CD_G_Instructions.CDG_Border_Preset:
                    Console.WriteLine("Border preset.");
                    break;
                case (byte)CD_G_Instructions.CDG_Define_Transparent:
                    Console.WriteLine("Define transparent.");
                    break;
                case (byte)CD_G_Instructions.CDG_Load_Color_Table_0_7:
                    Console.WriteLine("Load Color Table (0 to 7).");
                    break;
                case (byte)CD_G_Instructions.CDG_Load_Color_Table_8_15:
                    Console.WriteLine("Load Color Table (8 to 15).");
                    break;
                case (byte)CD_G_Instructions.CDG_Memory_Preset:
                    Console.WriteLine("Memory preset.");
                    break;
                case (byte)CD_G_Instructions.CDG_Scroll_Copy:
                    Console.WriteLine("Scroll copy.");
                    break;
                case (byte)CD_G_Instructions.CDG_Scroll_Preset:
                    Console.WriteLine("Scroll preset.");
                    break;
                case (byte)CD_G_Instructions.CDG_Tile_Block:
                    Console.WriteLine("Tile block.");
                    break;
                case (byte)CD_G_Instructions.CDG_Tile_Block_XOR:
                    Console.WriteLine("Tile block. (XOR)");
                    break;
                default:
                    Console.WriteLine("Unknown instruction 0x{0:X2}", packet.instruction);
                    break;
            }

            Console.WriteLine("P Parity = 0x{0:X2}{1:X2}{2:X2}{3:X2}", packet.parityP[0], packet.parityP[1], packet.parityP[2], packet.parityP[3]);
            Console.WriteLine("Q Parity = 0x{0:X2}{1:X2}", packet.parityQ[0], packet.parityQ[1]);
        }
    }
}

