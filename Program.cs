//
//  Program.cs
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
using System.IO;

namespace SubChannelDecoder
{
    class MainClass
    {
        const int QQuadraphonic = 0x80;
        const int QData = 0x40;
        const int QCopyPermitted = 0x20;
        const int QPreEmphasis = 0x10;
        const int QMode1 = 0x01;
        const int QMode2 = 0x02;
        const int QMode3 = 0x03;
        const int QMode5 = 0x05;

        static readonly char[] ISRCTable =
            {
            '0', '1', '2', '3', '4',
            '5', '6', '7', '8', '9',
            ' ', ' ', ' ', ' ', ' ',
            ' ', ' ', 'A', 'B', 'C',
            'D', 'E', 'F', 'G', 'H',
            'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R',
            'S', 'T', 'U', 'V', 'W',
            'X', 'Y', 'Z'
        };

        static readonly char[] BCDTable =
        {
            '0', '1', '2', '3',
            '4', '5', '6', '7',
            '8', '9', 'A', 'B',
            'C', 'D', 'E', 'F'
        };

        public static void Main(string[] args)
        {
            Console.WriteLine("SubChannelDecoder 0.04");
            Console.WriteLine("© 2015 Natalia Portillo");
            Console.WriteLine();

            if (args.Length != 2 && args.Length != 1)
            {
                Usage();
                return;
            }

            try
            {
                if(!File.Exists(args[0]))
                {
                    Console.WriteLine("Specified file does not exist");
                    Usage();
                    return;
                }

                FileStream fs = new FileStream(args[0], FileMode.Open, FileAccess.Read);

                if(args.Length == 1)
                {
                    bool? interleaved = null;

                    int sectors = (int)(fs.Length / 96);
                    for(int sector = 0; sector < sectors; sector++)
                    {
                        byte[] sectorBytes = new byte[96];

                        fs.Seek(sector*96, SeekOrigin.Begin);
                        fs.Read(sectorBytes, 0, 96);

                        if(interleaved == null)
                        {
                            if(CheckQCRC(DeinterleaveSubchannel(sectorBytes).q))
                            {
                                Console.WriteLine("Subchannel is interleaved.");
                                interleaved = true;
                            }
                            else if(CheckQCRC(UnpackSubchannel(sectorBytes).q))
                            {
                                Console.WriteLine("Subchannel is not interleaved.");
                                interleaved = true;
                            }
                        }

                        Subchannel sub = UnpackSubchannel(sectorBytes, interleaved);

                        Console.WriteLine("Sector {0}", sector);
                        Console.WriteLine("\tP: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                            sub.p[0], sub.p[1], sub.p[2], sub.p[3], sub.p[4], sub.p[5],
                            sub.p[6], sub.p[7], sub.p[8], sub.p[9], sub.p[10], sub.p[11]);

                        Console.WriteLine("\tQ: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                            sub.q[0], sub.q[1], sub.q[2], sub.q[3], sub.q[4], sub.q[5],
                            sub.q[6], sub.q[7], sub.q[8], sub.q[9], sub.q[10], sub.q[11]);

                        Console.WriteLine("\tR: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                            sub.r[0], sub.r[1], sub.r[2], sub.r[3], sub.r[4], sub.r[5],
                            sub.r[6], sub.r[7], sub.r[8], sub.r[9], sub.r[10], sub.r[11]);

                        Console.WriteLine("\tS: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                            sub.s[0], sub.s[1], sub.s[2], sub.s[3], sub.s[4], sub.s[5],
                            sub.s[6], sub.s[7], sub.s[8], sub.s[9], sub.s[10], sub.s[11]);

                        Console.WriteLine("\tT: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                            sub.t[0], sub.t[1], sub.t[2], sub.t[3], sub.t[4], sub.t[5],
                            sub.t[6], sub.t[7], sub.t[8], sub.t[9], sub.t[10], sub.t[11]);

                        Console.WriteLine("\tU: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                            sub.u[0], sub.u[1], sub.u[2], sub.u[3], sub.u[4], sub.u[5],
                            sub.u[6], sub.u[7], sub.u[8], sub.u[9], sub.u[10], sub.u[11]);

                        Console.WriteLine("\tV: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                            sub.v[0], sub.v[1], sub.v[2], sub.v[3], sub.v[4], sub.v[5],
                            sub.v[6], sub.v[7], sub.v[8], sub.v[9], sub.v[10], sub.v[11]);

                        Console.WriteLine("\tW: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                            sub.w[0], sub.w[1], sub.w[2], sub.w[3], sub.w[4], sub.w[5],
                            sub.w[6], sub.w[7], sub.w[8], sub.w[9], sub.w[10], sub.w[11]);

                        PrintQSubchannel(sub.q);

                        if(interleaved == true || interleaved == null)
                        {
                            if((sectorBytes[0] & 0x3F) == 0x09 ||
                                (sectorBytes[24] & 0x3F) == 0x09 ||
                                (sectorBytes[48] & 0x3F) == 0x09 ||
                                (sectorBytes[72] & 0x3F) == 0x09)
                            {
                                Console.WriteLine("CD+G detected.");
                                CD_G.PrintCDGPackets(sectorBytes);
                            }
                        }
                        else
                        {
                            byte[] interBytes = InterleaveSubchannel(sub);

                            if((interBytes[0] & 0x3F) == 0x09 ||
                                (interBytes[24] & 0x3F) == 0x09 ||
                                (interBytes[48] & 0x3F) == 0x09 ||
                                (interBytes[72] & 0x3F) == 0x09)
                            {
                                Console.WriteLine("CD+G detected.");
                                CD_G.PrintCDGPackets(interBytes);
                            }
                        }
                    }
                }
                else
                {
                    bool? interleaved = null;

                    int sector;
                    if(!int.TryParse(args[1], out sector))
                    {
                        Console.WriteLine("Specified sector is not a number");
                        Usage();
                        return;
                    }

                    if((sector*96) >= fs.Length)
                    {
                        Console.WriteLine("Specified sector is bigger than specified file");
                        Usage();
                        return;
                    }

                    byte[] sectorBytes = new byte[96];

                    fs.Seek(sector*96, SeekOrigin.Begin);
                    fs.Read(sectorBytes, 0, 96);

                    if(interleaved == null)
                    {
                        if(CheckQCRC(DeinterleaveSubchannel(sectorBytes).q))
                        {
                            Console.WriteLine("Subchannel is interleaved.");
                            interleaved = true;
                        }
                        else if(CheckQCRC(UnpackSubchannel(sectorBytes).q))
                        {
                            Console.WriteLine("Subchannel is not interleaved.");
                            interleaved = true;
                        }
                    }

                    Subchannel sub = UnpackSubchannel(sectorBytes, interleaved);

                    Console.WriteLine("Sector {0}", sector);
                    Console.WriteLine("\tP: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                        sub.p[0], sub.p[1], sub.p[2], sub.p[3], sub.p[4], sub.p[5],
                        sub.p[6], sub.p[7], sub.p[8], sub.p[9], sub.p[10], sub.p[11]);

                    Console.WriteLine("\tQ: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                        sub.q[0], sub.q[1], sub.q[2], sub.q[3], sub.q[4], sub.q[5],
                        sub.q[6], sub.q[7], sub.q[8], sub.q[9], sub.q[10], sub.q[11]);
                    
                    Console.WriteLine("\tR: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                        sub.r[0], sub.r[1], sub.r[2], sub.r[3], sub.r[4], sub.r[5],
                        sub.r[6], sub.r[7], sub.r[8], sub.r[9], sub.r[10], sub.r[11]);
                    
                    Console.WriteLine("\tS: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                        sub.s[0], sub.s[1], sub.s[2], sub.s[3], sub.s[4], sub.s[5],
                        sub.s[6], sub.s[7], sub.s[8], sub.s[9], sub.s[10], sub.s[11]);
                    
                    Console.WriteLine("\tT: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                        sub.t[0], sub.t[1], sub.t[2], sub.t[3], sub.t[4], sub.t[5],
                        sub.t[6], sub.t[7], sub.t[8], sub.t[9], sub.t[10], sub.t[11]);
                    
                    Console.WriteLine("\tU: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                        sub.u[0], sub.u[1], sub.u[2], sub.u[3], sub.u[4], sub.u[5],
                        sub.u[6], sub.u[7], sub.u[8], sub.u[9], sub.u[10], sub.u[11]);
                    
                    Console.WriteLine("\tV: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                        sub.v[0], sub.v[1], sub.v[2], sub.v[3], sub.v[4], sub.v[5],
                        sub.v[6], sub.v[7], sub.v[8], sub.v[9], sub.v[10], sub.v[11]);
                    
                    Console.WriteLine("\tW: 0x{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                        sub.w[0], sub.w[1], sub.w[2], sub.w[3], sub.w[4], sub.w[5],
                        sub.w[6], sub.w[7], sub.w[8], sub.w[9], sub.w[10], sub.w[11]);

                    PrintQSubchannel(sub.q);

                    if(interleaved == true || interleaved == null)
                    {
                        if((sectorBytes[0] & 0x3F) == 0x09 ||
                            (sectorBytes[24] & 0x3F) == 0x09 ||
                            (sectorBytes[48] & 0x3F) == 0x09 ||
                            (sectorBytes[72] & 0x3F) == 0x09)
                        {
                            Console.WriteLine("CD+G detected.");
                        }
                    }
                    else
                    {
                        byte[] interBytes = InterleaveSubchannel(sub);

                        if((interBytes[0] & 0x3F) == 0x09 ||
                            (interBytes[24] & 0x3F) == 0x09 ||
                            (interBytes[48] & 0x3F) == 0x09 ||
                            (interBytes[72] & 0x3F) == 0x09)
                        {
                            Console.WriteLine("CD+G detected.");
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Some error happened while reading file");
                throw;
            }
        }

        public static void PrintQSubchannel(byte[] q)
        {
            if ((q[0] & 0x0F) == QData)
            {
                Console.WriteLine("Sector is part of a data track");
                if ((q[0] & QPreEmphasis) == QPreEmphasis)
                    Console.WriteLine("Track has been recorded incrementally");
                if ((q[0] & QQuadraphonic) == QQuadraphonic)
                    Console.WriteLine("Track is for broadcsting use");
            }
            else
            {
                if ((q[0] & QPreEmphasis) == QPreEmphasis)
                    Console.WriteLine("Track has pre-emphasis");
                if ((q[0] & QQuadraphonic) == QQuadraphonic)
                    Console.WriteLine("Track contains quadraphonic audio");
            }


            if ((q[0] & QCopyPermitted) == QCopyPermitted)
                Console.WriteLine("Track may be copied");

            if ((q[0] & 0x0F) == QMode1)
            {
                int hour = 0;
                int phour = 0;

                if (q[6] != 0)
                {
                    Console.WriteLine("DDCD");
                    phour = q[6] & 0x0F;
                    hour = (q[6] & 0xF0) >> 4;
                }

                Console.WriteLine("Q Mode 1: Address Marks / TOC");
                if (q[1] == 0x00)
                {
                    Console.WriteLine("\tLead-in");

                    if (hour != 0)
                        Console.WriteLine("\tRelative Address {3:X1}:{0:X2}:{1:X2}:{2:X2}", q[3], q[4], q[5], hour);
                    else
                        Console.WriteLine("\tRelative Address {0:X2}:{1:X2}:{2:X2}", q[3], q[4], q[5]);

                    switch (q[2])
                    {
                        case 0xA0:
                            {
                                Console.WriteLine("\tFirst data track is track {0:X2}", q[7]);
                                Console.WriteLine("\tDisc type: {0}", q[8]);
                                break;
                            }
                        case 0xA1:
                            {
                                Console.WriteLine("\tLast data track is track {0:X2}", q[7]);
                                break;
                            }
                        case 0xA2:
                            {
                                if (phour != 0)
                                    Console.WriteLine("\tLead-Out starts at Address {3:X1}:{0:X2}:{1:X2}:{2:X2}", q[7], q[8], q[9], phour);
                                else
                                    Console.WriteLine("\tLead-Out starts at Address {0:X2}:{1:X2}:{2:X2}", q[7], q[8], q[9]);
                                break;
                            }
                        case 0xF0:
                            {
                                Console.WriteLine("Book type: {0}", q[7]);
                                Console.WriteLine("Material type: {0}", q[8]);
                                Console.WriteLine("Moment of inertia: {0}", q[9]);
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("\tTrack {0:X}", q[2]);
                                if (phour != 0)
                                    Console.WriteLine("\tTrack Starting Address {3:X1}:{0:X2}:{1:X2}:{2:X2}", q[7], q[8], q[9], phour);
                                else
                                    Console.WriteLine("\tTrack Starting Address {0:X2}:{1:X2}:{2:X2}", q[7], q[8], q[9]);
                                break;
                            }
                    }
                }
                else
                {
                    if (q[1] == 0xAA)
                        Console.WriteLine("\tLead-out");
                    else
                        Console.WriteLine("\tTrack {0:X}", q[1]);

                    Console.WriteLine("\tIndex {0:X}", q[2]);
                    if (hour != 0)
                        Console.WriteLine("\tRelative Address {3:X1}:{0:X2}:{1:X2}:{2:X2}", q[3], q[4], q[5], hour);
                    else
                        Console.WriteLine("\tRelative Address {0:X2}:{1:X2}:{2:X2}", q[3], q[4], q[5]);

                    if (phour != 0)
                        Console.WriteLine("\tAbsolute Address {3:X1}:{0:X2}:{1:X2}:{2:X2}", q[7], q[8], q[9], phour);
                    else
                        Console.WriteLine("\tAbsolute Address {0:X2}:{1:X2}:{2:X2}", q[7], q[8], q[9]);
                }
            }
            else if ((q[0] & 0x0F) == QMode2)
            {
                Console.WriteLine("Q Mode 2: Media Catalog Number");

                Console.WriteLine("Catalog number: {0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X1}", q[1], q[2], q[3], q[4], q[5], q[6], (q[7] & 0xF0) >> 4);

                if ((q[7] & 0xF) != 0x00 || q[6] != 0x00)
                    Console.WriteLine("Zero = 0x{0:X}{1:X2}", (q[7] & 0xF), q[8]);

                Console.WriteLine("\tAbsolute Frame {0:X2}", q[9]);
            }
            else if ((q[0] & 0x0F) == QMode3)
            {
                Console.WriteLine("Q Mode 3: ISRC");

                char[] isrc = new char[12];

                isrc[0] = ISRCTable[(q[1] & 0xFC) >> 2];
                isrc[1] = ISRCTable[((q[1] & 0x03) << 4) + ((q[2] & 0xF0) >> 4)];
                isrc[2] = ISRCTable[((q[2] & 0x0F) << 2) + ((q[3] & 0xC0) >> 6)];
                isrc[3] = ISRCTable[(q[3] & 0x3F)];
                isrc[4] = ISRCTable[(q[4] & 0xFC) >> 2];

                isrc[5] = BCDTable[(q[5] & 0xF0) >> 4];
                isrc[6] = BCDTable[(q[5] & 0xF)];
                isrc[7] = BCDTable[(q[6] & 0xF0) >> 4];
                isrc[8] = BCDTable[(q[6] & 0xF)];
                isrc[9] = BCDTable[(q[7] & 0xF0) >> 4];
                isrc[10] = BCDTable[(q[7] & 0xF)];
                isrc[11] = BCDTable[(q[8] & 0xF0) >> 4];

                Console.WriteLine("ISRC: {0}", new string(isrc));
                Console.WriteLine("\tAbsolute Frame {0:X2}", q[9]);
            }
            else if ((q[0] & 0x0F) == QMode5)
            {
                Console.WriteLine("Q Mode 5: Recordable information");

                int hour = 0;
                int phour = 0;

                if (q[6] != 0)
                {
                    Console.WriteLine("DDCD");
                    phour = q[6] & 0x0F;
                    hour = (q[6] & 0xF0) >> 4;
                }

                switch (q[2])
                {
                    case 0xB0:
                        {
                            if (hour != 0)
                                Console.WriteLine("\tStart of next possible program in the recordable area of the disc {3:X1}:{0:X2}:{1:X2}:{2:X2}", q[3], q[4], q[5], hour);
                            else
                                Console.WriteLine("\tStart of next possible program in the recordable area of the disc {0:X2}:{1:X2}:{2:X2}", q[3], q[4], q[5]);
                        
                            if (phour != 0)
                                Console.WriteLine("\tMaximum start of outermost Lead-out in the recordable area of the disc {3:X1}:{0:X2}:{1:X2}:{2:X2}", q[7], q[8], q[9], phour);
                            else
                                Console.WriteLine("\tMaximum start of outermost Lead-out in the recordable area of the disc {0:X2}:{1:X2}:{2:X2}", q[7], q[8], q[9]);

                            break;
                        }
                    case 0xB1:
                        {
                            Console.WriteLine("{0:X2} skip interval pointers", q[7]);
                            Console.WriteLine("{0:X2} skip track pointers", q[8]);
                            break;
                        }
                    case 0xB2:
                    case 0xB3:
                    case 0xB4:
                        {
                            Console.WriteLine("Skip track {0:X2}", q[3]);
                            Console.WriteLine("Skip track {0:X2}", q[4]);
                            Console.WriteLine("Skip track {0:X2}", q[5]);
                            Console.WriteLine("Skip track {0:X2}", q[6]);
                            Console.WriteLine("Skip track {0:X2}", q[7]);
                            Console.WriteLine("Skip track {0:X2}", q[8]);
                            Console.WriteLine("Skip track {0:X2}", q[9]);
                            break;
                        }
                    case 0xC0:
                        {
                            Console.WriteLine("Optimum recording power: 0x{0:X2}", q[3]);
                            if (phour != 0)
                                Console.WriteLine("Start time of the first Lead-in area in the disc: {3:X2}:{0:X2}:{1:X2}:{2:X2}", q[7], q[8], q[9], phour);
                            else
                                Console.WriteLine("Start time of the first Lead-in area in the disc: {0:X2}:{1:X2}:{2:X2}", q[7], q[8], q[9]);
                            break;
                        }
                    case 0xC1:
                        {
                            Console.WriteLine("Copy of information of A1 from ATIP found");
                            Console.WriteLine("Min = {0}", q[3]);
                            Console.WriteLine("Sec = {0}", q[4]);
                            Console.WriteLine("Frame = {0}", q[5]);
                            Console.WriteLine("Zero = {0}", q[6]);
                            Console.WriteLine("PMIN = {0}", q[7]);
                            Console.WriteLine("PSEC = {0}", q[8]);
                            Console.WriteLine("PFRAME = {0}", q[9]);
                            break;
                        }
                    case 0xCF:
                        {
                            if (phour != 0)
                                Console.WriteLine("Start position of outer part lead-in area: {3:X2}:{0:X2}:{1:X2}:{2:X2}", q[7], q[8], q[9], phour);
                            else
                                Console.WriteLine("Start position of outer part lead-in area: {0:X2}:{1:X2}:{2:X2}", q[7], q[8], q[9]);

                            if (hour != 0)
                                Console.WriteLine("Stop position of inner part lead-out area: {3:X2}:{0:X2}:{1:X2}:{2:X2}", q[3], q[4], q[5], hour);
                            else
                                Console.WriteLine("Stop position of inner part lead-out area: {0:X2}:{1:X2}:{2:X2}", q[3], q[4], q[5]);
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Start time for interval that should be skipped: {0:X2}:{1:X2}:{2:X2}", q[7], q[8], q[9]);
                            Console.WriteLine("Ending time for interval that should be skipped: {0:X2}:{1:X2}:{2:X2}", q[3], q[4], q[5]);
                            break;
                        }
                }
            }
            else
                Console.WriteLine("Unknown Q Mode {0}", (q[0] & 0x0F));

            if (CheckQCRC(q))
                Console.WriteLine("Q CRC = 0x{0:X2}{1:X2} (OK)", q[10], q[11]);
            else
                Console.WriteLine("Q CRC = 0x{0:X2}{1:X2} (BAD)", q[10], q[11]);
        }

        public static bool CheckQCRC(byte[] q)
        {
            byte[] QCalculatedCrc;
            CRC16CCITTContext.Data(q, 10, out QCalculatedCrc);

            return q[10] == QCalculatedCrc[0] && q[11] == QCalculatedCrc[1];
        }

        public static Subchannel UnpackSubchannel(byte[] subchannel, bool? interleaved)
        {
            if (interleaved == null || interleaved == true)
                return DeinterleaveSubchannel(subchannel);
            return UnpackSubchannel(subchannel);
        }

        public static Subchannel UnpackSubchannel(byte[] subchannel)
        {
            Subchannel sub = new Subchannel();

            Array.Copy(subchannel, 0, sub.p, 0, 12);
            Array.Copy(subchannel, 12, sub.q, 0, 12);
            Array.Copy(subchannel, 24, sub.r, 0, 12);
            Array.Copy(subchannel, 36, sub.s, 0, 12);
            Array.Copy(subchannel, 48, sub.t, 0, 12);
            Array.Copy(subchannel, 60, sub.u, 0, 12);
            Array.Copy(subchannel, 72, sub.v, 0, 12);
            Array.Copy(subchannel, 84, sub.w, 0, 12);

            return sub;
        }

        public static Subchannel DeinterleaveSubchannel(byte[] subchannel)
        {
            Subchannel sub = new Subchannel();

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    sub.p[i] += ShiftRight((byte)(subchannel[j + i * 8] & 0x80), j);
                    sub.q[i] += ShiftRight((byte)(subchannel[j + i * 8] & 0x40), j-1);
                    sub.r[i] += ShiftRight((byte)(subchannel[j + i * 8] & 0x20), j-2);
                    sub.s[i] += ShiftRight((byte)(subchannel[j + i * 8] & 0x10), j-3);
                    sub.t[i] += ShiftRight((byte)(subchannel[j + i * 8] & 0x8), j-4);
                    sub.u[i] += ShiftRight((byte)(subchannel[j + i * 8] & 0x4), j-5);
                    sub.v[i] += ShiftRight((byte)(subchannel[j + i * 8] & 0x2), j-6);
                    sub.w[i] += ShiftRight((byte)(subchannel[j + i * 8] & 0x1), j-7);
                }
            }

            return sub;
        }

        public static byte[] InterleaveSubchannel(Subchannel sub)
        {
            byte[] subchannel = new byte[96];

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    subchannel[j + i * 8] += ShiftLeft((byte)(sub.p[i] & (0x80 >> j)), j);
                    subchannel[j + i * 8] += ShiftLeft((byte)(sub.q[i] & (0x80 >> j)), j-1);
                    subchannel[j + i * 8] += ShiftLeft((byte)(sub.r[i] & (0x80 >> j)), j-2);
                    subchannel[j + i * 8] += ShiftLeft((byte)(sub.s[i] & (0x80 >> j)), j-3);
                    subchannel[j + i * 8] += ShiftLeft((byte)(sub.t[i] & (0x80 >> j)), j-4);
                    subchannel[j + i * 8] += ShiftLeft((byte)(sub.u[i] & (0x80 >> j)), j-5);
                    subchannel[j + i * 8] += ShiftLeft((byte)(sub.v[i] & (0x80 >> j)), j-6);
                    subchannel[j + i * 8] += ShiftLeft((byte)(sub.w[i] & (0x80 >> j)), j-7);
                }
            }

            return subchannel;
        }

        public static byte ShiftRight(byte value, int shifted)
        {
            return shifted < 0 ? (byte)(value << Math.Abs(shifted)) : (byte)(value >> shifted);
        }

        public static byte ShiftLeft(byte value, int shifted)
        {
            return shifted < 0 ? (byte)(value >> Math.Abs(shifted)) : (byte)(value << shifted);
        }

        public static void Usage()
        {
            Console.WriteLine();
            Console.WriteLine("SubChannelDecoder <subchannel.sub> [sector]");
            Console.WriteLine();
            Console.WriteLine("\t\tsubchannel.sub: Subchannel file");
            Console.WriteLine("\t\tsector: Counting sector 0 as start of file, subchannel sector to decode");
        }
    }
}
