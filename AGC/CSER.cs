using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace AGC
{
    class CSER
    {
        public Dictionary<string, object> cse(Vector<double> r0, Vector<double> v0, double dt, double mu, Dictionary<string, double> last)
        {
            double dtcp;

            if (last["dtcp"] == 0)
            {
                dtcp = dt;
            } else
            {
                dtcp = last["dtcp"];
            }

            double xcp = last["xcp"];
            double x = xcp;
            double A = last["A"];
            double D = last["D"];
            double E = last["E"];
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

            double n = 0d;
            double r0m = Math.Abs(
                    Math.Sqrt(
                        Math.Pow(r0.At(0), 2) +
                        Math.Pow(r0.At(1), 2) +
                        Math.Pow(r0.At(2), 2)
                    )
                );
            double f1 = f0 * Math.Sqrt(r0m / mu);
            double f2 = 1 / f1;
            double f3 = f2 / r0m;
            double f4 = f1 * r0m;
            double f5 = f0 / Math.Sqrt(r0m);
            double f6 = f0 * Math.Sqrt(r0m);

            Vector<double> ir0 = r0 / r0m;
            Vector<double> v0s = v0 * f1;
            double sigma0s = ir0.DotProduct(v0s);
            double b0 = Math.Sqrt(
                Math.Abs(
                    Math.Sqrt(
                        Math.Pow(v0s.At(0), 2) +
                        Math.Pow(v0s.At(1), 2) +
                        Math.Pow(v0s.At(2), 2)
                    )
                )) - 1;
            double alphas = 1 - b0;

            double xguess = f5 * x;
            double xlast = f5 * xcp;
            double xmin = 0;
            double dts = f3 * dt;
            double dtlast = f3 * dtcp;
            double dtmin = 0;
            double dtmax = 0;
            double xP = 0;
            double Ps = 0;
            List<double> pack;

            double xmax = 2 * Math.PI / Math.Sqrt(Math.Abs(alphas));

            if (alphas > 0)
            {
                dtmax = xmax / alphas;
                xP = xmax;
                Ps = dtmax;

                while (dts >= Ps)
                {
                    n = n + 1;
                    dts = dts - Ps;
                    dtlast = dtlast - Ps;
                    xguess = xguess - xP;
                    xlast = xlast - xP;
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
                        xmin = xmax;
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
            double dtguess = pack[0];

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
                    xmin = xlast;
                    dtmin = dtlast;
                }
            }

            pack = kil(imax, dts, xguess, dtguess, xmin, dtmin, xmax, dtmax, sigma0s, alphas, kmax, A, D, E);
            xguess = pack[0];
            dtguess = pack[1];
            A = pack[2];
            D = pack[3];
            E = pack[4];

            double rs = 1 + 2 * (b0 * A + sigma0s * D * E);
            double b4 = 1 / rs;
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

            last["dtcp"] = dtc;
            last["xcp"] = xc;
            last["A"] = A;
            last["D"] = D;
            last["E"] = E;

            double F = 1 - 2 * A;
            double Gs = 2 * (D * E + sigma0s * A);
            double Fts = -2 * b4 * D * E;
            double Gt = 1 - 2 * b4 * A;

            Vector<double> r = r0m * (F * ir0 + Gs * v0s);
            Vector<double> v = f2 * (Fts * ir0 + Gt * v0s);

            return new Dictionary<string, object>()
            {
                {"r", r },
                {"v", v },
                {"last", last }
            };
        }

        private List<double> ktti(double xarg, double s0s, double a, double kmax)
        {
            double u1 = uss(xarg, a, kmax);
            double zs = 2 * u1;
            double E = 1 - 0.5 * a * Math.Pow(zs, 2);
            double W = Math.Sqrt(Math.Max(0.5 + E / 2, 0));
            double D = W * zs;
            double A = Math.Pow(D, 2);
            double B = 2 * (E + s0s * D);
            double Q = qfc(W);
            double t = D * (B + A * Q);

            List<double> output = new List<double>();
            output.Add(t);
            output.Add(A);
            output.Add(D);
            output.Add(E);
            return output;
        }

        private double uss(double xarg, double a, double kmax)
        {
            double du1 = xarg / 4;
            double u1 = du1;
            double u1old;
            double f7 = -a * Math.Pow(du1, 2);
            double k = 3d;

            while (k < kmax)
            {
                du1 = f7 * du1 / (k * (k - 1));
                u1old = u1;
                u1 = u1 + du1;

                if (u1 == u1old)
                {
                    break;
                }

                k = k + 2;
            }

            return u1;
        }

        private double qfc(double w)
        {
            double xq = 0d;
            double y = (w - 1) / (w + 1);

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

            double j = Math.Floor(xq);
            double b = y / (1 + (j - 1) / (j - 2) * (1 - 0));

            while (j > 2)
            {
                j = j - 1;
                b = y / (1 + (j - 1) / (j + 2) * (1 - b));
            }

            return 1 / Math.Pow(w, 2) * (1 + (2 - b / 2) / (2 * w * (w + 1)));
        }

        private List<double> kil(double imax, double dts, double xguess, double dtguess, double xmin, double dtmin, double xmax, double dtmax, double s0s, double a_, double kmax, double A, double D, double E)
        {
            double etp = 0.000001;
            double i = 0d;
            double dterror;
            double dxs = 0d;
            double xold = 0d;
            double dtold = 0d;

            while (i < imax)
            {
                dterror = dts - dtguess;

                if (Math.Abs(dterror) < etp)
                {
                    break;
                }

                List<double> pack = si(dterror, xguess, dtguess, xmin, dtmin, xmax, dtmax);
                dxs = pack[0];
                xmin = pack[1];
                dtmin = pack[2];
                xmax = pack[3];
                dtmax = pack[4];
                pack.Clear();

                xold = xguess;
                xguess = xguess + dxs;

                if (xguess == xold)
                {
                    break;
                }

                dtold = dtguess;
                pack = ktti(xguess, s0s, a_, kmax);
                dtguess = pack[0];
                A = pack[1];
                D = pack[2];
                E = pack[3];

                if (dtguess == dtold)
                {
                    break;
                }

                i = i + 1;
            }

            List<double> output = new List<double>();
            output.Add(xguess);
            output.Add(dtguess);
            output.Add(A);
            output.Add(D);
            output.Add(E);

            return output;
        }

        private List<double> si(double dterror, double xguess, double dtguess, double xmin, double dtmin, double xmax, double dtmax)
        {
            double etp = 0.000001d;
            double dxs;
            double dtminp = dtguess - dtmin;
            double dtmaxp = dtguess - dtmax;

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

            List<double> output = new List<double>();
            output.Add(dxs);
            output.Add(xmin);
            output.Add(dtmin);
            output.Add(xmax);
            output.Add(dtmax);

            return output;
        }
    }
}
