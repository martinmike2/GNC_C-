using System;

namespace AGC.Utilities
{
    public class AgcTuple
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        
        public AgcTuple(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Tuple<double, double, double>(AgcTuple x)
        {
            return new Tuple<double, double, double>(x.X, x.Y, x.Z);
        }

        public static implicit operator AgcTuple(Tuple<double, double, double> x)
        {
            return new AgcTuple(x.Item1, x.Item2, x.Item3);
        }

        public static AgcTuple operator *(AgcTuple left, AgcTuple right)
        {
            return AgcMath.cross(left, right);
        }

        public static AgcTuple operator *(AgcTuple left, double right)
        {
            return new AgcTuple(
                left.X * right,
                left.Y * right,
                left.Z * right
                );
        }
        
        public static AgcTuple operator *(double right, AgcTuple left)
        {
            return new AgcTuple(
                left.X * right,
                left.Y * right,
                left.Z * right
            );
        }
        
        public static AgcTuple operator /(AgcTuple left, AgcTuple right)
        {
            return new AgcTuple(
                left.X / right.X,
                left.Y / right.Y,
                left.Z / right.Z
                );
        }

        public static AgcTuple operator /(AgcTuple left, double right)
        {
            return new AgcTuple(
                left.X / right,
                left.Y / right,
                left.Z / right
            );
        }
        
        public static AgcTuple operator /(double right, AgcTuple left)
        {
            return new AgcTuple(
                left.X / right,
                left.Y / right,
                left.Z / right
            );
        }

        public static AgcTuple operator +(AgcTuple left, AgcTuple right)
        {
            return new AgcTuple(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z
                );
        }

        public static AgcTuple operator +(AgcTuple left, double right)
        {
            return new AgcTuple(
                left.X + right,
                left.Y + right,
                left.Z + right
                );
        }
        
        public static AgcTuple operator +(double right, AgcTuple left)
        {
            return new AgcTuple(
                left.X + right,
                left.Y + right,
                left.Z + right
            );
        }
        
        public static AgcTuple operator -(AgcTuple left, AgcTuple right)
        {
            return new AgcTuple(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z
                );
        }

        public static AgcTuple operator -(AgcTuple left, double right)
        {
            return new AgcTuple(
                left.X - right,
                left.Y - right,
                left.Z - right
                );
        }
        
        public static AgcTuple operator -(double right, AgcTuple left)
        {
            return new AgcTuple(
                left.X - right,
                left.Y - right,
                left.Z - right
            );
        }
    }
}