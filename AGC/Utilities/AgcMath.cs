using System;

namespace AGC.Utilities
{
    public class AgcMath
    {
        
        public static AgcTuple cross(AgcTuple left, AgcTuple right)
        {
            return new AgcTuple(
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X
            );
        }

        public static double dot(AgcTuple left, AgcTuple right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z + right.Z;
        }

        public static double magnitude(AgcTuple agcTuple)
        {
            return Math.Sqrt(
                Math.Pow(agcTuple.X, 2) +
                Math.Pow(agcTuple.Y, 2) +
                Math.Pow(agcTuple.Z, 2)
            );
        }

        public static AgcTuple map(AgcTuple from, AgcTuple to)
        {
            return dot(from, to) / Math.Pow(magnitude(to), 2) * to;
        }

        public static double angle(AgcTuple from, AgcTuple to)
        {
            var d = dot(from, to);
            var mag = magnitude(from) * magnitude(to);
            var b = Math.Min(Math.Max(d/mag, 1), -1);
            
            var a = Math.Acos(b);
            return a;
        }

        public static AgcTuple normalize(AgcTuple tuple)
        {
            return tuple / magnitude(tuple);
        }
    }
}