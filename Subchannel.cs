//
//  Subchannel.cs
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

namespace SubChannelDecoder
{
    public struct Subchannel
    {
        public byte[] p;
        public byte[] q;
        public byte[] r;
        public byte[] s;
        public byte[] t;
        public byte[] u;
        public byte[] v;
        public byte[] w;

        public Subchannel()
        {
            p = new byte[12];
            q = new byte[12];
            r = new byte[12];
            s = new byte[12];
            t = new byte[12];
            u = new byte[12];
            v = new byte[12];
            w = new byte[12];
        }
    }
}

