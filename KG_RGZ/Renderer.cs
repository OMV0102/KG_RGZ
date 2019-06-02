using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KG_RGZ
{
    public class Renderer
    {
        private GLControl _g;
        private Color BackgroundColor = Color.White;
        private Color GridColor = Color.DarkGray;
        private Color Graph1Color = Color.Red;
        private Color Point1Color = Color.Black;

        public readonly float ZoomIn = 1.2f;
        public readonly float ZoomOut = 0.83333f; // это 5/6 или 1/1.2

        private int CellSize = 1;
        private int xl, yl;

        private int xm { get { return xl + _g.Width; } }
        private int ym { get { return yl + _g.Height; } }

        public float Scale = 30f;
        public float TranslateX = 0;
        public float TranslateY = 0;

        public Renderer(GLControl g)
        {
            this._g = g;
            xl = -_g.Width / 2;
            yl = -_g.Height / 2;
            // начальная позиция холста
            TranslateX = _g.Width / 2 / Scale;
            TranslateY = -_g.Height / 2 / Scale;
        }

        public void Initialize()
        {
            // Настройка OpenGL
            GL.ClearColor(BackgroundColor); // установить цвет фона области

            // 2D mode by Hemul1995
            GL.Viewport(_g.ClientSize); // натягивает изображение на все окно
            // установить масштаб изображения 1:1
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            // устанавливает систему координат
            // ось x оставляем как есть, ось y переворачиваем
            GL.Ortho(0, _g.Width, -_g.Height, 0, -1, 1); // magic numbers
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            // Конец настройки OpenGL
        }

        public void Refresh()
        {
            _g.Invalidate();
        }

        public void ScaleIn(int delta = 1)
        {
            Scale *= (float)Math.Pow(ZoomIn, delta);
        }

        public void ScaleOut(int delta = 1)
        {
            if (Scale <= 2) return;
            Scale *= (float)Math.Pow(ZoomOut, delta);
        }

        public void Move(int dx, int dy)
        {
            TranslateX -= (float) dx / Scale;
            TranslateY += (float) dy / Scale;
        }

        public void Begin()
        {
            // ColorBuffer - буфер цвета. 
            //     С каждым пикселем на экране связано значение цвета, которое записывается в буфере цвета.
            // DepthBuffer - буфер глубины. 
            //     Там хранятся соответствия, какой объект ближе и должен отображаться, а какой дальше и нет.
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //GL.PushMatrix();
            GL.LoadIdentity(); // это важно! если не загружать единичную матрицу каждый раз, будут глитчи
            GL.Scale(Scale, Scale, 1);
            GL.Translate(TranslateX, TranslateY, 0);
        }

        public void End()
        {
            //GL.PopMatrix();
        }

        /// <summary>
        /// Рисование координатной сетки.
        /// </summary>
        public void DrawGrid()
        {
            // задаем ширину линии
            GL.LineWidth(1);
            // устанавливаем цвет
            GL.Color3(GridColor);
            // начинаем рисовать
            GL.Begin(PrimitiveType.Lines);
            // расставляем точки и рисуем линии
            for (int i = xl - xl % CellSize; i < xm + xm % CellSize; i += CellSize)
            { // вертикальные
                GL.Vertex2(i, yl);
                GL.Vertex2(i, ym);
            }
            for (int i = yl - yl % CellSize; i < ym + ym % CellSize; i += CellSize)
            { // горизонтальные
                GL.Vertex2(xl, i);
                GL.Vertex2(xm, i);
            }
            // заканчиваем рисовать
            GL.End();
        }

        public void DrawAxis()
        {
            GL.LineWidth(3);
            GL.Color3(GridColor);
            GL.Begin(PrimitiveType.Lines);
            // Ox
            GL.Vertex2(xl, 0);
            GL.Vertex2(xm, 0);
            // Oy
            GL.Vertex2(0, yl);
            GL.Vertex2(0, ym);
            GL.End();
        }

        public void DrawSpline(Spline s)
        {
            GL.LineWidth(2);
            GL.Color3(Graph1Color);
            GL.Begin(PrimitiveType.LineStrip);
            foreach (var pt in s.PointsView)
            {
                GL.Vertex2(pt.X, pt.Y);
            }
            GL.End();
        }

        public void DrawPoints(Spline s)
        {
            GL.Color3(Point1Color);
            GL.Begin(PrimitiveType.Quads);
            foreach (var pt in s.Points)
            {
                GL.Vertex2(pt.X - 0.1, pt.Y + 0.1);
                GL.Vertex2(pt.X - 0.1, pt.Y - 0.1);
                GL.Vertex2(pt.X + 0.1, pt.Y - 0.1);
                GL.Vertex2(pt.X + 0.1, pt.Y + 0.1);
            }
            GL.End();
        }
    }
}
