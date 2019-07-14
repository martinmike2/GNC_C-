using System;
using AGC.Data;
using AGC.Utilities;
using System.Collections.Generic;

namespace AGC.Guidance
{
    public class UPFG
    {
        public static void upfg(Vehicle vehicle = null)
        {
            if (vehicle is null)
            {
                vehicle = Globals.VehicleData;
            }

            var gamma = Globals.TargetData.Angle;
            var iy = Globals.TargetData.Normal;
            var rdval = Globals.TargetData.Radius;
            var vdval = Globals.TargetData.Velocity;
            var t = Globals.VehicleSate.Time;
            var m = Globals.VehicleSate.Mass;
            var r = Globals.VehicleSate.Radius;
            var v = Globals.VehicleSate.Velocity;
            var cser = Globals.PreviousUpfgState.Cser;
            var rbias = Globals.PreviousUpfgState.Rbias;
            var rd = Globals.PreviousUpfgState.Rd;
            var rgrav = Globals.PreviousUpfgState.Rgrav;
            var tp = Globals.PreviousUpfgState.Time;
            var vprev = Globals.PreviousUpfgState.Velocity;
            var vgo = Globals.PreviousUpfgState.Vgo;

            var SM = new List<int>();
            var aL = new List<double>();
            var md = new List<double>();
            var ve = new List<double>();
            var fT = new List<double>();
            var aT = new List<double>();
            var tu = new List<double>();
            var tb = new List<double>();

            foreach (var stage in vehicle.Stages)
            {
                SM.Add(stage.UpfgMode);
                aL.Add(stage.GLim * Globals.G0);

                var pack = stage.getThrust(Globals.G0);
                fT.Add(pack[0]);
                md.Add(pack[1]);
                ve.Add(pack[2] * Globals.G0);
                aT.Add(fT[fT.Count - 1] / stage.MassTotal);
                tu.Add(ve[ve.Count - 1] / aT[aT.Count - 1]);
            }

            var dt = t - tp;
            var dvsensed = v - vprev;
            vgo -= dvsensed;

            tb[0] -= Globals.PreviousUpfgState.Tb;

            switch (SM[0])
            {
                case 1:
                    aT[0] = fT[0] / m;
                    break;
                case 2:
                    aT[0] = aL[0];
                    break;
            }

            tu[0] = ve[0] / aT[0];

            double L = 0;
            var Li = new List<double>();

            var newVehicle = new Vehicle();
            for (var i = 0; i < vehicle.Stages.Count; i++)
            {
                switch (SM[i])
                {
                    case 1:
                        Li.Add(ve[i] * Math.Log(tu[i] / (tu[i] - tb[i])));
                        break;
                    case 2:
                        Li.Add(aL[i] * tb[i]);
                        break;
                    default:
                        Li.Add(0);
                        break;
                }

                L += Li[i];

                if (!(L < AgcMath.magnitude(vgo))) continue;

                var stages = vehicle.Stages.GetRange(0, vehicle.Stages.Count - 1);
                newVehicle.Stages = stages;
                upfg(newVehicle);
            }

            Li.Add(AgcMath.magnitude(vgo) - L);

            var tgoi = new List<double>();

            for (var i = 0; i < vehicle.Stages.Count; i++)
            {
                switch (SM[i])
                {
                    case 1:
                        tb[i] = tu[i] * (1 - Math.Pow(Math.E, (-Li[i] / ve[i])));
                        break;
                    case 2:
                        tb[i] = Li[i] / aL[i];
                        break;
                }

                if (i == 0)
                {
                    tgoi.Add(tb[i]);
                }
                else
                {
                    tgoi.Add(tgoi[i - 1] + tb[i]);
                }
            }

            var L1 = Li[0];
            var tgo = tgoi[vehicle.Stages.Count - 1];

            L = 0;
            double J = 0;
            double S = 0;
            double Q = 0;
            double H = 0;
            double P = 0;
            var Ji = new List<double>();
            var Si = new List<double>();
            var Qi = new List<double>();
            var Pi = new List<double>();
            double tgoi1 = 0;

            for (var i = 0; i < vehicle.Stages.Count; i++)
            {
                if (i > 0)
                {
                    tgoi1 = tgoi[i - 1];
                }

                switch (SM[i])
                {
                    case 1:
                        Ji.Add(tu[i] * Li[i] - ve[i] * tb[i]);
                        Si.Add(-Ji[i] + tb[i] * Li[i]);
                        Qi.Add(Si[i] * (tu[i] + tgoi1) - 0.5 * ve[i] * Math.Pow(tb[i], 2));
                        Pi.Add(Qi[i] * (tu[i] + tgoi1) - 0.5 * ve[i] * Math.Pow(tb[i], 2) * (tb[i] / 3 + tgoi1));
                        break;
                    case 2:
                        Ji.Add(0.5 * Li[i] * tb[i]);
                        Si.Add(Ji[i]);
                        Qi.Add(Si[i] * (tb[i] / 3 + tgoi1));
                        Pi.Add((1 / 6) * Si[i] * (Math.Pow(tgoi[i], 2) + 2 * tgoi[i] * tgoi1 + 3 * Math.Pow(tgoi1, 2)));
                        break;
                }

                Ji[i] += Li[i] * tgoi1;
                Si[i] += L * tb[i];
                Qi[i] += J * tb[i];
                Pi[i] += H * tb[i];

                L += Li[i];
                J += Ji[i];
                S += Si[i];
                Q += Qi[i];
                P += Pi[i];
                H = J * tgoi[i] - Q;
            }

            var lambda = AgcMath.normalize(vgo);

            if (Globals.PreviousUpfgState.Tgo > 0)
            {
                rgrav *= Math.Pow(tgo / Globals.PreviousUpfgState.Tgo, 2);

            }

            var rgo = rd - (r + v * tgo - rgrav);
            var iz = AgcMath.normalize(AgcMath.cross(rd, iy));
            var rgoxy = rgo - AgcMath.dot(iz, rgo) * iz;
            var rgoz = S - AgcMath.dot(lambda, rgoxy) / AgcMath.dot(lambda, iz);

            rgo = rgoxy + rgoz * iz + rbias;

            var lambdade = Q - S * J / L;
            var lambdadot = (rgo - S * lambda) / lambdade;
            var iF_ = AgcMath.normalize(lambda - lambdadot * J / L);
            var phi = AgcMath.angle(iF_, lambda) * (Math.PI / 180);
            var phidot = -phi * L / J;
            var vthrust = (L - 0.5 * L * Math.Pow(phi, 2) - J * phi * phidot - 0.5 * H * Math.Pow(phidot, 2)) * lambda;
            var rthrust_i = S - 0.5 * S * Math.Pow(phi, 2) - Q * phi * phidot - 0.5 * P * Math.Pow(phidot, 2);
            var rthrust = rthrust_i * lambda - (S * phi + Q * phidot) * AgcMath.normalize(lambdadot);
            var vbias = vgo - vthrust;
            rbias = rgo - rthrust;

            var up = AgcMath.normalize(r);
            var east = AgcMath.normalize(AgcMath.cross(new AgcTuple(0, 0, 1), up));
            var pitch = AgcMath.angle(iF_, up);
            var inplane = AgcMath.map(up, iF_);
            var yaw = AgcMath.angle(up, east);
            var tangent = AgcMath.cross(up, east);

            if (AgcMath.dot(inplane, tangent) < 0)
            {
                yaw = -yaw;
            }

            var rc1 = r - 0.1 * rthrust / tgo - (tgo / 30) * vthrust;
            var vc1 = v + 1.2 * rthrust / tgo - 0.1 * vthrust;

            var cserPack = Cser.cse(rc1, vc1, tgo, cser);
            cser = cserPack.Previous;
            rgrav = cserPack.Radius - rc1 - vc1 * tgo;
            var vgrav = cserPack.Velocity - vc1;

            var rp = r + v * tgo + rgrav + rthrust;
            rp -= AgcMath.dot(rp, iy) * iy;
            rd = rdval * AgcMath.normalize(rp);
            var ix = AgcMath.normalize(rd);
            iz = AgcMath.cross(ix, iy);

            var vv1 = new AgcTuple(ix.X, iy.X, iz.X);
            var vv2 = new AgcTuple(ix.Y, iy.Y, iz.Y);
            var vv3 = new AgcTuple(ix.Z, iy.Z, iz.Z);

            var vop = new AgcTuple(
                Math.Sin(gamma),
                0,
                Math.Cos(gamma)

            );

            var vd = new AgcTuple(
                AgcMath.dot(vv1, vop),
                AgcMath.dot(vv2, vop),
                AgcMath.dot(vv3, vop)
            );

            vd *= vdval;

            vgo = vd - v - vgrav - vbias;


            Globals.PreviousUpfgState = Globals.CurrentUpfgState;
            Globals.CurrentUpfgState = new UpfgState(cser, rbias, rd, rgrav, t, v, vgo,
                Globals.PreviousUpfgState.Tb + dt, tgo, dt);
            Globals.VehicleGuidance = new Data.Guidance(iF_, pitch, yaw, 0, 0, tgo);
        }
    }
}