using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rysoinator
{
    class RealImaginator : Imaginator
    {
        private Figuinator figuinator;
        private Matrix<double> rotation_matrix;
        private Matrix<double> translation_matrix;
        private Matrix<double> resize_matrix;

        private Matrix<double> transformation_matrix;


        public RealImaginator(Figuinator f)
        {
            figuinator = f;
            resize_matrix = Matrix<double>.Build.Diagonal(4, 4, 1);
            translation_matrix = Matrix<double>.Build.DenseOfDiagonalArray(new double[] {1,1,1,1});
            rotation_matrix = Matrix<double>.Build.DenseOfDiagonalArray(new double [] {1,1,1,1});
            UpdateTransformationMatrix();
        }

        public override Bitmap GenerateImage(int width, int height, int level)
        {
            return figuinator.GenerateImage(width, height, level);
        }

        public override void Resize(double s)
        {
            Matrix<double> tmp = Matrix<double>.Build.Diagonal(4, 4, s);
            tmp[3, 3] = 0;
            resize_matrix += tmp;
            if (resize_matrix[0, 0] <= 0)
            {
                resize_matrix -= tmp;
            }
            UpdateTransformationMatrix();
        }

        public override void Rotate(double x, double y)
        {
            Matrix<double> rot_x = Matrix<double>.Build.DenseOfDiagonalArray(new double[] { 1, 1, 1, 1 });
            Matrix<double> rot_y = Matrix<double>.Build.DenseOfDiagonalArray(new double[] { 1, 1, 1, 1 });
            rot_x[1, 1] = Math.Cos(y);
            rot_x[2, 1] = -Math.Sin(y);
            rot_x[1, 2] = Math.Sin(y);
            rot_x[2, 2] = Math.Cos(y);

            rot_y[0, 0] = Math.Cos(-x);
            rot_y[2, 2] = Math.Cos(-x);
            rot_y[2, 0] = Math.Sin(-x);
            rot_y[0, 2] = -Math.Sin(-x);

            rotation_matrix = rot_x * rot_y * rotation_matrix;

            UpdateTransformationMatrix();
        }

        public override void RotateScene(double x, double y)
        {

        }

        public override void SetM(double m)
        {
            figuinator.SetM(m);
        }

        public override void SetRadiuses(double a, double b, double c)
        {
            figuinator.SetRadiuses(a, b, c);
        }

        public override void Translate(double x, double y)
        {
            Vector<double> vec = Vector<double>.Build.Dense(new double[] { x, y, 0, 0 });
            translation_matrix[0, 3] += vec[0];
            translation_matrix[1, 3] += vec[1];
            translation_matrix[2, 3] += vec[2];

            UpdateTransformationMatrix();
        }

        public override void TranslateScene(double x, double y)
        {
            
        }

        private void UpdateTransformationMatrix()
        {
            transformation_matrix = resize_matrix * translation_matrix * rotation_matrix;
            figuinator.SetTransformationMatrix(transformation_matrix);
        }
    }
}
