using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rysoinator
{
    class RealElipsoinator : Figuinator
    {
        private Matrix<double> d;
        private Matrix<double> m;
        private Matrix<double> d_m;

        private double r_a, r_b, r_c;

        private Vector<double> observator_position;

        private double m_col;

        public RealElipsoinator()
        {
            d = Matrix<double>.Build.DiagonalOfDiagonalArray(new double[] { 0.0001, 0.00002, 0.000005, -1 });
            m = Matrix<double>.Build.DiagonalOfDiagonalArray(new double[] { 1, 1, 1, 1 });
            d_m = d;
            observator_position = Vector<double>.Build.DenseOfArray(new double[] { 0, 0, 1500 });
            m_col = 1;
        }

        public override Bitmap GenerateImage(int width, int height, int level)
        {
            double A = d_m[0, 0], B = d_m[1, 0], C = d_m[2, 0], D = d_m[3, 0], E = d_m[0, 1], F = d_m[1, 1],
                G = d_m[1, 2], H = d_m[1, 3], I = d_m[2, 0], J = d_m[2, 1], K = d_m[2, 2], L = d_m[2, 3],
                M = d_m[3, 0], N = d_m[3, 1], P = d_m[3, 2], Q = d_m[3, 3];
            double a = K;
            width = (int)((double)width / (level * level));
            height = (int)((double)height / (level * level));
            Bitmap resultImage = new Bitmap(width, height);
            BmpPixelSnoop snoop = new BmpPixelSnoop(resultImage);
            {
                Color lastColor;
                for (int x_r = -width / 2; x_r < width / 2; x_r++)
                {
                    int x = x_r * level * level;
                    for (int y_r = -height / 2; y_r < height / 2; y_r++)
                    {
                        snoop.SetPixel(x_r + width / 2, y_r + height / 2, Color.DarkGray);
                        int y = y_r * level * level;
                        double b = x * I + y * J + x * C + y * G + P + L;
                        double c = x * x * A + x * y * E + x * M + x * y * B +
                            y * y * F + y * N + x * D + y * H + Q;

                        double delta = b * b - 4 * a * c;

                        if (delta < 0)
                        {
                            lastColor = Color.DarkGray;
                        }
                        else
                        {
                            double z;
                            if (delta == 0)
                            {
                                z = -b / 2 * a;
                            }
                            else
                            {
                                double z1 = (-b - Math.Sqrt(delta)) / (2 * a);
                                double z2 = (-b + Math.Sqrt(delta)) / (2 * a);

                                if (observator_position[2] - z1 < observator_position[2] - z2)
                                    z = z1;
                                else
                                    z = z2;
                            }
                            Vector<double> normal = Vector<double>.Build.DenseOfArray(
                                   new double[] {
                                    2 * x * A + y * E + z * I + M + y * B + z * C + D,
                                    x * E + x * B + 2 * y * F + z * J + N + z * G + H,
                                    x * I + y * J + x * C + y * G + 2 * z * K + P + L }).Normalize(2);
                            Vector<double> to_observer = (observator_position - Vector<double>.Build.DenseOfArray(new double[] { x, y, z })).Normalize(2);

                            double color = to_observer.DotProduct(normal);
                            color = Math.Pow(color, m_col);
                            if (color < 0) color = 0;
                            lastColor = Color.FromArgb((byte)(Color.Yellow.R * color), (byte)(Color.Yellow.G * color), (byte)(Color.Yellow.B * color));
                        }
                        snoop.SetPixel(x_r + width / 2, y_r + height / 2, lastColor);
                    }
                }
            }
            return resultImage;
        }

        public override void SetM(double m)
        {
            m_col = m;
        }

        public override void SetRadiuses(double a, double b, double c)
        {
            double[] initial = { 1 / (a * a), 1 / (b * b), 1 / (c * c), -1 };
            r_a = a;
            r_b = b;
            r_c = c;
            d = Matrix<double>.Build.DiagonalOfDiagonalArray(initial);
            d_m = m.Transpose() * d * m;
        }

        public override void SetTransformationMatrix(Matrix<double> m_)
        {
            m = m_.Inverse();
            d_m = m.Transpose() * d * m;
        }

        
    }
}
