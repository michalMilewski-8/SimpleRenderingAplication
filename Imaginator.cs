using System;
using System.Drawing;

namespace rysoinator
{
    abstract class Imaginator
    {
        abstract public void Rotate (double x, double y);
        abstract public void RotateScene (double x, double y);
        abstract public void Translate (double x, double y);
        abstract public void TranslateScene (double x, double y);
        abstract public void Resize (double s);
        public abstract void SetM(double m);
        abstract public void SetRadiuses (double a, double b, double c);
        abstract public Bitmap GenerateImage(int width, int height, int level);
    }
}
