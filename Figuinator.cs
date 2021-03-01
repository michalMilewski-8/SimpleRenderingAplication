using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rysoinator
{
    abstract class Figuinator
    {
        public abstract void SetRadiuses(double a, double b, double c);
        public abstract void SetM(double m);
        public abstract void SetTransformationMatrix(Matrix<double> m);
        public abstract Bitmap GenerateImage(int width, int height, int level);
    }
}
