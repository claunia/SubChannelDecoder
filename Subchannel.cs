using System;

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
            this.p = new byte[12];
            this.q = new byte[12];
            this.r = new byte[12];
            this.s = new byte[12];
            this.t = new byte[12];
            this.u = new byte[12];
            this.v = new byte[12];
            this.w = new byte[12];
        }
    }
}

