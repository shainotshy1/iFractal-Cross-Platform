using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaSharpFormsDemos
{
    public class Complex
    {
        private readonly double re;
        private readonly double im;

        public Complex(double real, double imag)
        {
            re = real;
            im = imag;
        }

        public double Abs()
        {
            return Math.Sqrt(re*re+im*im);
        }

        public double Phase()
        {
            return Math.Atan2(im, re);
        }

        public Complex Plus(Complex b)
        {
            Complex a = this;             
            double real = a.re + b.re;
            double imag = a.im + b.im;
            return new Complex(real, imag);
        }
        public Complex Minus(Complex b)
        {
            Complex a = this;
            double real = a.re - b.re;
            double imag = a.im - b.im;
            return new Complex(real, imag);
        }
        public Complex Times(Complex b)
        {
            Complex a = this;
            double real = a.re * b.re - a.im * b.im;
            double imag = a.re * b.im + a.im * b.re;
            return new Complex(real, imag);
        }
        public Complex Scale(double alpha)
        {
            return new Complex(alpha * re, alpha * im);
        }
        public Complex Conjugate()
        {
            return new Complex(re, -im);
        }
        public Complex Reciprocal()
        {
            double scale = re * re + im * im;
            return new Complex(re / scale, -im / scale);
        }
        public Complex Divides(Complex b)
        {
            Complex a = this;
            return a.Times(b.Reciprocal());
        }
        public Complex Exp()
        {
            return new Complex(Math.Exp(re) * Math.Cos(im), Math.Exp(re) * Math.Sin(im));
        }
        public Complex Sin()
        {
            return new Complex(Math.Sin(re) * Math.Cosh(im), Math.Cos(re) * Math.Sinh(im));
        }
        public Complex Cos()
        {
            return new Complex(Math.Cos(re) * Math.Cosh(im), -Math.Sin(re) * Math.Sinh(im));
        }
        public Complex Tan()
        {
            return Sin().Divides(Cos());
        }
    }
}
