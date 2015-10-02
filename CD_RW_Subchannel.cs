//
//  CD_RW_Subchannel.cs
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
    public static class CD_RW_Subchannel
    {
        public struct CD_RW_Packet
        {
            public byte command;
            public byte instruction;
            public byte[] parityQ;
            public byte[] data;
            public byte[] parityP;
        }

        public enum CD_RW_Commands : byte
        {
            /// <summary>
            /// CD with line graphics
            /// </summary>
            CD_Line_Graphics = 0x08,
            /// <summary>
            /// CD+G
            /// </summary>
            CD_G = 0x09,
            /// <summary>
            /// CD+EG
            /// </summary>
            CD_EG = 0x0A,
            /// <summary>
            /// CD Text
            /// </summary>
            CD_Text = 0x14,
            /// <summary>
            /// CD+MIDI
            /// </summary>
            CD_MIDI = 0x18,
            /// <summary>
            /// ...
            /// </summary>
            CD_User = 0x38
        }

        public enum CD_LG_Instructions : byte
        {
            CDLG_Write_Font = 4,
            CDLG_Scroll_Screen = 12
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

        public enum CD_EG_Instructions : byte
        {
            CDEG_Memory_Control = 3,
            CDEG_Write_Additional_Ground = 6,
            CDEG_XOR_Additional_Font = 14,
            CDEG_Load_Color_Table_0_7 = 16,
            CDEG_Load_Color_Table_8_15,
            CDEG_Load_Color_Table_16_23,
            CDEG_Load_Color_Table_24_31,
            CDEG_Load_Color_Table_32_39,
            CDEG_Load_Color_Table_40_47,
            CDEG_Load_Color_Table_48_55,
            CDEG_Load_Color_Table_56_63,
            CDEG_Load_Color_Table_64_71,
            CDEG_Load_Color_Table_72_79,
            CDEG_Load_Color_Table_80_87,
            CDEG_Load_Color_Table_88_95,
            CDEG_Load_Color_Table_96_103,
            CDEG_Load_Color_Table_104_111,
            CDEG_Load_Color_Table_112_119,
            CDEG_Load_Color_Table_120_127,
            CDEG_Load_Color_Table_128_135,
            CDEG_Load_Color_Table_136_143,
            CDEG_Load_Color_Table_144_151,
            CDEG_Load_Color_Table_152_159,
            CDEG_Load_Color_Table_160_167,
            CDEG_Load_Color_Table_168_175,
            CDEG_Load_Color_Table_176_183,
            CDEG_Load_Color_Table_184_191,
            CDEG_Load_Color_Table_192_199,
            CDEG_Load_Color_Table_200_207,
            CDEG_Load_Color_Table_208_215,
            CDEG_Load_Color_Table_216_223,
            CDEG_Load_Color_Table_224_231,
            CDEG_Load_Color_Table_232_239,
            CDEG_Load_Color_Table_240_247,
            CDEG_Load_Color_Table_248_255,
            CDEG_Load_Color_Table_Additional_8_15,
            CDEG_Load_Color_Table_Additional_16_23,
            CDEG_Load_Color_Table_Additional_24_31,
            CDEG_Load_Color_Table_Additional_32_39,
            CDEG_Load_Color_Table_Additional_40_47,
            CDEG_Load_Color_Table_Additional_48_55,
            CDEG_Load_Color_Table_Additional_56_63,
            CDEG_Load_Color_Table_Additional_64_71,
            CDEG_Load_Color_Table_Additional_72_79,
            CDEG_Load_Color_Table_Additional_80_87,
            CDEG_Load_Color_Table_Additional_88_95,
            CDEG_Load_Color_Table_Additional_96_103,
            CDEG_Load_Color_Table_Additional_104_111,
            CDEG_Load_Color_Table_Additional_112_119,
            CDEG_Load_Color_Table_Additional_120_127,
            CDEG_Load_Color_Table_Additional_128_135,
            CDEG_Load_Color_Table_Additional_136_143,
            CDEG_Load_Color_Table_Additional_144_151,
            CDEG_Load_Color_Table_Additional_152_159,
            CDEG_Load_Color_Table_Additional_160_167,
            CDEG_Load_Color_Table_Additional_168_175,
            CDEG_Load_Color_Table_Additional_176_183,
            CDEG_Load_Color_Table_Additional_184_191,
            CDEG_Load_Color_Table_Additional_192_199,
            CDEG_Load_Color_Table_Additional_200_207,
            CDEG_Load_Color_Table_Additional_208_215,
            CDEG_Load_Color_Table_Additional_216_223,
            CDEG_Load_Color_Table_Additional_224_231,
            CDEG_Load_Color_Table_Additional_232_239,
            CDEG_Load_Color_Table_Additional_240_247,
            CDEG_Load_Color_Table_Additional_248_255
        }

        static readonly int[] offsets = 
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
                    ms.WriteByte(subchannels[(pack * 24) + offsets[column]]);

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

        public static void Print_CD_RW_Packets(byte[] subchannel)
        {
            Print_CD_RW_Packets(Packetize_CD_RW(subchannel));
        }

        public static void Print_CD_RW_Packets(CD_RW_Packet[] packets)
        {
            for (int i = 0; i < packets.Length; i++)
            {
                Console.WriteLine("CD RW subchannel Packet {0}", i);
                Print_CD_RW_Packet(packets[i]);
            }
        }

        public static void Print_CD_RW_Packet(CD_RW_Packet packet)
        {
            switch (packet.command)
            {
                case (byte)CD_RW_Commands.CD_Line_Graphics:
                    Console.Write("CD Line Graphics: ");
                    switch (packet.instruction)
                    {
                        case (byte)CD_LG_Instructions.CDLG_Scroll_Screen:
                            Console.WriteLine("Scroll screen.");
                            break;
                        case (byte)CD_LG_Instructions.CDLG_Write_Font:
                            Console.WriteLine("Write font.");
                            break;
                        default:
                            Console.WriteLine("Unknown instruction 0x{0:X2}", packet.instruction);
                            break;
                    }
                    break;
                case (byte)CD_RW_Commands.CD_G:
                    Console.Write("CD+G instruction: ");
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
                    break;
                case (byte)CD_RW_Commands.CD_EG:
                    Console.Write("CD+G instruction: ");
                    switch (packet.instruction)
                    {
                        case (byte)CD_EG_Instructions.CDEG_Memory_Control:
                            Console.WriteLine("Memory control.");
                            break;
                        case (byte)CD_EG_Instructions.CDEG_Write_Additional_Ground:
                            Console.WriteLine("Write additional font foreground/background.");
                            break;
                        case (byte)CD_EG_Instructions.CDEG_XOR_Additional_Font:
                            Console.WriteLine("XOR additional font.");
                            break;
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_0_7:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_8_15:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_16_23:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_24_31:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_32_39:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_40_47:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_48_55:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_56_63:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_64_71:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_72_79:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_80_87:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_88_95:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_96_103:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_104_111:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_112_119:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_120_127:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_128_135:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_136_143:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_144_151:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_152_159:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_160_167:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_168_175:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_176_183:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_184_191:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_192_199:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_200_207:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_208_215:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_216_223:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_224_231:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_232_239:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_240_247:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_248_255:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_8_15:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_16_23:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_24_31:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_32_39:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_40_47:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_48_55:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_56_63:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_64_71:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_72_79:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_80_87:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_88_95:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_96_103:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_104_111:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_112_119:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_120_127:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_128_135:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_136_143:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_144_151:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_152_159:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_160_167:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_168_175:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_176_183:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_184_191:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_192_199:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_200_207:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_208_215:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_216_223:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_224_231:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_232_239:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_240_247:
                        case (byte)CD_EG_Instructions.CDEG_Load_Color_Table_Additional_248_255:
                            Console.WriteLine("Load Color Table.");
                            break;
                        default:
                            Console.WriteLine("Unknown instruction 0x{0:X2}", packet.instruction);
                            break;
                    }
                    break;
                case (byte)CD_RW_Commands.CD_MIDI:
                    Console.Write("CD+MIDI contains {0} MIDI bytes.", packet.instruction & 0xF);
                    break;
                case (byte)CD_RW_Commands.CD_User:
                    Console.Write("User data packetized.");
                    break;
                default:
                    return;
            }

            Console.WriteLine("P Parity = 0x{0:X2}{1:X2}{2:X2}{3:X2}", packet.parityP[0], packet.parityP[1], packet.parityP[2], packet.parityP[3]);
            Console.WriteLine("Q Parity = 0x{0:X2}{1:X2}", packet.parityQ[0], packet.parityQ[1]);
        }
    }
}

