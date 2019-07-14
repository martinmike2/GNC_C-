using System;
using AGC.Data;
using AGC.Utilities;
using System.Collections.Generic;

namespace AGC.Guidance
{
    class Cser
    {
        public static CserState cse(AgcTuple r0, AgcTuple v0, double dt, CserState last)
        {
            double dtcp;

            if (Math.Abs(last.Dtcp) < 0.0000001)
            {
                dtcp = dt;
            } else
            {
                dtcp = last.Dtcp;
            }

            var xcp = last.Xcp;
            var x = xcp;
            var a = last.A;
            var d = last.D;
            var e = last.E;
            double kmax = 10;
            double imax = 10;
            double f0;

            if (dt >= 0)
            {
                f0 = 1;
            } else
            {
                f0 = -1;
            }

            var n = 0d;
            var r0m = Math.Abs(
                    AgcMath.magnitude(r0)
                );
            var f1 = f0 * Math.Sqrt(r0m / Globals.MU);
            var f2 = 1 / f1;
            var f3 = f2 / r0m;
            var f4 = f1 * r0m;
            var f5 = f0 / Math.Sqrt(r0m);
            var f6 = f0 * Math.Sqrt(r0m);

            var ir0 = r0 / r0m;
            var v0s = v0 * f1;
            var sigma0s = AgcMath.dot(ir0, v0s);
            var b0 = Math.Sqrt(
                Math.Abs(
                    AgcMath.magnitude(v0s)
                )) - 1;
            var alphas = 1 - b0;

            var xguess = f5 * x;
            var xlast = f5 * xcp;
            var xmin = 0;
            var dts = f3 * dt;
            var dtlast = f3 * dtcp;
            double dtmin = 0;
            double dtmax = 0;
            double xP = 0;
            double Ps = 0;
            List<double> pack;

            var xmax = 2 * Math.PI / Math.Sqrt(Math.Abs(alphas));

            if (alphas > 0)
            {
                dtmax = xmax / alphas;
                xP = xmax;
                Ps = dtmax;

                while (dts >= Ps)
                {
                    n += 1;
                    dts -= Ps;
                    dtlast -= Ps;
                    xguess -= xP;
                    xlast -= xP;
                }
            } else
            {
                pack = ktti(xmax, sigma0s, alphas, kmax);
                dtmax = pack[0];

                if (dtmax < dts)
                {
                    while (dtmax < dts)
                    {
                        dtmin = dtmax;
                        xmin = (int) xmax;
                        xmax = 2 * xmax;
                        pack = ktti(xmax, sigma0s, alphas, kmax);
                        dtmax = pack[0];
                    }
                }
            }

            if (xmin >= xguess || xguess >= xmax)
            {
                xguess = 0.5 * (xmin + xmax);
            }

            pack = ktti(xguess, sigma0s, alphas, kmax);
            var dtguess = pack[0];

            if (dts < dtguess)
            {
                if (xguess < xlast && xlast < xmax && dtguess < dtlast && dtlast < dtmax)
                {
                    xmax = xlast;
                    dtmax = dtlast;
                }
            } else
            {
                if (xmin < xlast && xlast < xguess && dtmin < dtlast && dtlast < dtguess)
                {
                    xmin = (int) xlast;
                    dtmin = dtlast;
                }
            }

            pack = kil(imax, dts, xguess, dtguess, xmin, dtmin, xmax, dtmax, sigma0s, alphas, kmax, a, d, e);
            xguess = pack[0];
            dtguess = pack[1];
            a = pack[2];
            d = pack[3];
            e = pack[4];

            var rs = 1 + 2 * (b0 * a + sigma0s * d * e);
            var b4 = 1 / rs;
            double xc = 0;
            double dtc = 0;

            if (n>0)
            {
                xc = f6 * (xguess + n * xP);
                dtc = f4 * (dtguess + n * Ps);
            } else
            {
                xc = f6 * xguess;
                dtc = f4 * dtguess;
            }

            var f = 1 - 2 * a;
            var gs = 2 * (d * e + sigma0s * a);
            var fts = -2 * b4 * d * e;
            var gt = 1 - 2 * b4 * a;

            var r = r0m * (f * ir0 + gs * v0s);
            var v = f2 * (fts * ir0 + gt * v0s);

            return new CserState(r, v, dtc, xc, a, d, e, last);
        }

        private static List<double> ktti(double xarg, double s0s, double a, double kmax)
        {
            var u1 = uss(xarg, a, kmax);
            var zs = 2 * u1;
            var E = 1 - 0.5 * a * Math.Pow(zs, 2);
            var W = Math.Sqrt(Math.Max(0.5 + E / 2, 0));
            var D = W * zs;
            var A = Math.Pow(D, 2);
            var B = 2 * (E + s0s * D);
            var Q = qfc(W);
            var t = D * (B + A * Q);

            var output = new List<double> {t, A, D, E};
            return output;
        }

        private static double uss(double xarg, double a, double kmax)
        {
            var du1 = xarg / 4;
            var u1 = du1;
            double u1old;
            var f7 = -a * Math.Pow(du1, 2);
            var k = 3d;

            while (k < kmax)
            {
                du1 = f7 * du1 / (k * (k - 1));
                u1old = u1;
                u1 += du1;

                if (Math.Abs(u1 - u1old) < 0.0000001)
                {
                    break;
                }

                k += 2;
            }

            return u1;
        }

        private static double qfc(double w)
        {
            double xq;
            var y = (w - 1) / (w + 1);

            if ( w < 1 )
            {
                xq = 21.04 - 13.04 * w;
            } else if (w < 4.625)
            {
                xq = (5 / 3) * (2 * w + 5);
            } else if (w < 13.846)
            {
                xq = (10 / 7) * (w + 12);
            } else if (w < 44)
            {
                xq = 0.5 * (w + 60);
            } else if (w < 100)
            {
                xq = 0.25 * (1 + 164);
            } else
            {
                xq = 70;
            }

            var j = Math.Floor(xq);
            var b = y / (1 + (j - 1) / (j - 2) * (1 - 0));

            while (j > 2)
            {
                j -= 1;
                b = y / (1 + (j - 1) / (j + 2) * (1 - b));
            }

            return 1 / Math.Pow(w, 2) * (1 + (2 - b / 2) / (2 * w * (w + 1)));
        }

        private static List<double> kil(double imax, double dts, double xguess, double dtguess, double xmin, double dtmin, double xmax, double dtmax, double s0s, double a_, double kmax, double A, double D, double E)
        {
            var etp = 0.000001;
            var i = 0d;
            double dterror;
            double dxs;
            double xold;
            double dtold;

            while (i < imax)
            {
                dterror = dts - dtguess;

                if (Math.Abs(dterror) < etp)
                {
                    break;
                }

                var pack = si(dterror, xguess, dtguess, xmin, dtmin, xmax, dtmax);
                dxs = pack[0];
                xmin = pack[1];
                dtmin = pack[2];
                xmax = pack[3];
                dtmax = pack[4];
                pack.Clear();

                xold = xguess;
                xguess += dxs;

                if (Math.Abs(xguess - xold) < 0.0000001)
                {
                    break;
                }

                dtold = dtguess;
                pack = ktti(xguess, s0s, a_, kmax);
                dtguess = pack[0];
                A = pack[1];
                D = pack[2];
                E = pack[3];

                if (Math.Abs(dtguess - dtold) < 0.0000001)
                {
                    break;
                }

                i += 1;
            }

            var output = new List<double>
            {
                xguess,
                dtguess,
                A,
                D,
                E
            };

            return output;
        }

        private static List<double> si(double dterror, double xguess, double dtguess, double xmin, double dtmin, double xmax, double dtmax)
        {
            var etp = 0.000001d;
            double dxs;
            var dtminp = dtguess - dtmin;
            var dtmaxp = dtguess - dtmax;

            if(Math.Abs(dtminp) < etp || Math.Abs(dtmaxp) < etp)
            {
                dxs = 0;
            } else
            {
                if (dterror < 0)
                {
                    dxs = (xguess - xmax) * (dterror / dtmaxp);
                    if (xguess+dxs <= xmin)
                    {
                        dxs = (xguess - xmin) * (dterror / dtminp);
                    }
                    xmax = xguess;
                    dtmax = dtguess;
                } else
                {
                    dxs = (xguess - xmin) * (dterror / dtminp);

                    if (xguess + dxs >= xmax)
                    {
                        dxs = (xguess - xmax) * (dterror / dtmaxp);
                    }

                    xmin = xguess;
                    dtmin = dtguess;
                }
            }

            var output = new List<double>
            {
                dxs,
                xmin,
                dtmin,
                xmax,
                dtmax
            };

            return output;
        }
    }
}
