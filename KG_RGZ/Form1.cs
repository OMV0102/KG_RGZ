﻿using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;

namespace KG_RGZ
{
    public partial class Form1
    {
        private bool is_gl_load = false;
        private Renderer r;
        private Spline s;
        /// <summary>
        /// Точка клика мыши.
        /// </summary>
        private Point _mouse1, _mouse2;
        // Флаг
        private bool _isMoving = false;

        public Form1()
        {
            InitializeComponent();
            r = new Renderer(glControl1);
            s = new Spline();
            //s.LoadFromFile("input.txt");
        }

        // ======================================================================================================================

        #region События GLControl
        /// <summary>
        /// Загрузка внутренней области
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void glControl1_Load(object sender, EventArgs e)
        {
            ((GLControl)sender).MakeCurrent();
            r.Initialize();

            is_gl_load = true;
        }

        /// <summary>
        /// Изменение размера внутренней области
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        void glControl1_Resize(object sender, System.EventArgs e)
        {
            if (is_gl_load == true)
                r.Initialize();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (is_gl_load == true)
            {
                ((GLControl)sender).MakeCurrent();
                //frameNum++;

                r.Begin();
                r.DrawGrid();
                r.DrawAxis();
                r.DrawSpline(s);
                r.DrawPoints(s);
                r.End();

                // finalization
                ((GLControl)sender).SwapBuffers();
            }
        }
        #endregion

        // ======================================================================================================================

        #region События мыши
        /// <summary>
        /// Обработка клика мыши во внутренней области
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        _isMoving = true;
                        _mouse1 = e.Location;
                        break;
                    } 

                case MouseButtons.Left:
                    {
                        // TODO: move points
                        _mouse2 = e.Location;
                        //r.Refresh(); // no need now
                        break;
                    }
            }
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            switch (e.Button) {
                case MouseButtons.Right:
                    {
                        if (_isMoving)
                        {
                            if (Math.Abs(_mouse1.X - e.X) > r.Scale || Math.Abs(_mouse1.Y - e.Y) > r.Scale)
                            {
                                r.Move(_mouse1.X - e.X, _mouse1.Y - e.Y);
                                _mouse1 = e.Location;
                                r.Refresh();
                            }
                        }
                        break;
                    }
                case MouseButtons.Left:
                    {
                        // TODO: move points
                        r.Refresh();
                        break;
                    }
            }
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        _isMoving = false;
                        break;
                    }

                case MouseButtons.Left:
                    {
                        var pt = new PointF(e.X / r.Scale - r.TranslateX, -e.Y / r.Scale - r.TranslateY);
                        s.AddPoint(pt);
                        r.Refresh();
                        break;
                    }

                default:
                    break;
            }
        }

        private void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (is_gl_load == true)
            {
                if (e.Delta < 0)
                {
                    r.ScaleOut();
                }
                else
                {
                    r.ScaleIn();
                }
                r.Refresh();
            }
        }

        #endregion

        // ======================================================================================================================

        #region События клавиатуры
        /// <summary>
        /// Реагирование фигуры на нажатие клавиш
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void Form_KeyDown(object sender, KeyEventArgs e) {
            bool handled = true;
            switch (e.KeyCode) {
                case Keys.Q: r.ScaleOut(); break;
                case Keys.E: r.ScaleIn(); break;
                case Keys.Delete: s.Clear(); break;
                default: handled = false; break;
            }
            if(handled) r.Refresh();
        }
        #endregion

        // ======================================================================================================================

        #region События элементов формы
        /// <summary>
        /// Регулировка размера псевдопикселей
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void ChangeQuality(object sender, EventArgs e)
        {
            label2.Text = sizeTrackbar.Value.ToString();
            s.Steps = sizeTrackbar.Value;
            r.Refresh();
        }

        /// <summary>
        /// Смена цвета
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void ChangeColor(object sender, EventArgs e)
        {
            using (var d = new ColorDialog())
            {
                if (DialogResult.OK == d.ShowDialog())
                {
                    //r.ChangeColor(chooseFigureId.SelectedIndex, d.Color);
                    r.Refresh();
                }
            }
        }
        #endregion

        // ======================================================================================================================
    }
}
