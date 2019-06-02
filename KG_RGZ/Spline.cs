using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KG_RGZ {
    public class Spline {
        private char[] _spl = {' ', '\n', '\r', '\t'}; // separators
        private List<SplineFunction> sp;
        private List<PointF> _points;
        private List<PointF> _pointsView;
        public List<PointF> d_points;
        public List<PointF> i_points;

        private int _steps = 10;

        public int Steps {
            get { return _steps; }
            set { _steps = Math.Abs(value); ApplyChanges(); }
        }

        public List<PointF> Points { get { return _points; } }
        public List<PointF> PointsView { get { return _pointsView; } }

        public Spline() {
            sp = new List<SplineFunction>();
            _points = new List<PointF>();
            _pointsView = new List<PointF>(); // to avoid null exception
            d_points = new List<PointF>();
            i_points = new List<PointF>();
        }

        #region Работа с файлами

        private void DumpToFile() {
            var f = new StreamWriter("R:\\debug_cs.txt");
            foreach (var pf in sp) {
                f.WriteLine("{0} {1} {2} {3} {4} {5}", pf.A, pf.B, pf.C, pf.D, pf.H, pf.X);
            }
            f.Close();
        }

        public void LoadFromFile(string filename) {
            try {
                var f = new StreamReader(filename);
                PointF pt;
                while (!f.EndOfStream) {
                    string str = f.ReadLine();
                    string[] data = str.Split(_spl, StringSplitOptions.RemoveEmptyEntries);
                    float x = float.Parse(data[0]);
                    float y = float.Parse(data[1]);

                    _points.Add(pt = new PointF(x, y));
                }
                f.Close();
            }
            catch (Exception e) {
                MessageBox.Show(e.Message + "\n\n" + e.StackTrace, "Error durind loading");
            }
            ApplyChanges();
        }

        public void SaveToFile(string file1 = "answer.txt", string file2 = "text.txt") {
            try {
                var f = new StreamWriter(file1);
                foreach (var pf in sp) {
                    f.WriteLine("{0} {1} {2} {3} {4}", pf.A, pf.B, pf.C, pf.D, pf.X);
                }
                f.Close();
                f = new StreamWriter(file2);
                double ds = sp[0].X;

                for (int i = 1; i < sp.Count; i++) {
                    for (int j = 0; j < 5; j++) {
                        ds += 0.2;
                        f.WriteLine("{0}", Func(i, ds));
                    }
                }
                f.Close();
            }
            catch (Exception e) {
                MessageBox.Show(e.Message + "\n\n" + e.StackTrace, "Error durind saving");
            }
        }

        #endregion

        #region Матчасть

        private void CreateSF() {
            sp = new List<SplineFunction>();
            SplineFunction prev = null;
            foreach (var pt in _points) {
                var sp1 = new SplineFunction {
                    //P = pt,
                    X = pt.X,
                    A = pt.Y
                };
                if (_points.IndexOf(pt) > 0) {
                        sp1.H = pt.X - prev.X;
                }
                sp.Add(sp1);
                prev = sp1;
            }

            if (sp.Count == 0)
                throw new InvalidOperationException("List is empty");
            sp[0].C = 0;
            sp[sp.Count - 1].C = 0;
        }

    public void CountC() {
            // TODO: avoid exception if sp.Count <= 1
            float A = 0, B = 0, C = 0, F = 0;
            float[] a = new float[sp.Count - 1];
            float[] b = new float[sp.Count - 1];
            a.Initialize(); // заполнить массивы нуликами
            b.Initialize();
            for (int i = 1; i < sp.Count - 1; i++) {
                A = sp[i].H;
                B = sp[i + 1].H;
                float dy = sp[i].A - sp[i - 1].A;
                float dy1 = sp[i + 1].A - sp[i].A;
                C = 2 * (A + B);
                F = 6 * (dy1 / B - dy / A);
                float z = (A * a[i - 1] + C);
                a[i] = -B / z;
                b[i] = (F - A * b[i - 1]) / z;
            }
            sp[sp.Count - 2].C = (F - A * b[sp.Count - 2]) / (C + A * a[sp.Count - 2]);
            //Обратный ход нахождения коэффициентов с, он же x
            for (int i = sp.Count - 2; i >= 1; i--) {
                sp[i].C = a[i] * sp[i + 1].C + b[i];
            }
        }

        private void CountBAD() {
            sp[0].B = 0;
            sp[0].D = 0;
            for (int i = 1, j = 0; i < sp.Count; i++, j++) {
                sp[i].D = (sp[i].C - sp[j].C);
                sp[i].D /= sp[i].H;
                sp[i].B = ((sp[i].A - sp[j].A) / sp[i].H) + (sp[i].H * (2 * sp[i].C + sp[j].C) / 6);
            }
            for (int i = 0; i < sp.Count; i++) {
                sp[i].C /= 2;
                sp[i].D /= 6;
            }
        }

        private void CalculatePoints() {
            int n = _steps * sp.Count;
            _pointsView = new List<PointF>(n);
            _pointsView.Add(new PointF(sp[0].X, sp[0].Func()));
            i_points = new List<PointF>(n);
            d_points = new List<PointF>(n);
            SplineFunction pf_prev = null;
            float y = 0, y0 = 0, y1;
            foreach (var pf in sp) {
                if (sp.IndexOf(pf) > 0) {
                    float part = (pf.X - pf_prev.X) / _steps; //длина шага на i-ом сплайне
                    float X = pf_prev.X;
                    y1 = pf.I_Func(X);
                    for (int j = 0; j < _steps; j++) {
                        X += part;
                        y = pf.Func(X);
                        _pointsView.Add(new PointF(X, y));
                        //if (j < _steps - 1) {
                            y = pf.D_Func(X);
                            d_points.Add(new PointF(X, y));
                        y = -y1 + y0 + pf.I_Func(X);// - part*(j+1));
                            i_points.Add(new PointF(X, y));
                        //}
                    }
                    y0 = y;
                }
                pf_prev = pf;
            }
        }
#endregion

        private double Func(int i, double ds) {
            double ret = sp[i].A;
            ret += sp[i].B * (ds - sp[i].X);
            ret += sp[i].C * (ds - sp[i].X) * (ds - sp[i].X);
            ret += sp[i].D * (ds - sp[i].X) * (ds - sp[i].X) * (ds - sp[i].X);
            return ret;
        }

        public static double Func(double a, double b, double c, double d, double x, double s) {
            double an = a;
            an += b * (s - x);
            an += c * (s - x) * (s - x);
            an += d * (s - x) * (s - x) * (s - x);
            return an;
        }

        private void ApplyChanges() {
            if (_points.Count < 2) return;
            _points.Sort((x,y) => x.X.CompareTo(y.X));
            //sp.Sort((x,y) => x.X.CompareTo(y.X));
            CreateSF();
            CountC();
            CountBAD();
            CalculatePoints();
        }

        private static int SortByX(PointF p1, PointF p2) {
            if (p1.X > p2.X) return 1;
            if (p1.X < p2.X) return -1;
            if (p1.X.Equals(p2.X)) return p1.Y < p2.Y ? -1 : 1;
            return 0;
        }

        public void AddPoint(PointF pt) {
            int id;
            if (!(id = _points.FindIndex(x => x.X.Equals(pt.X))).Equals(-1))
            {
                _points[id] = new PointF(pt.X, pt.Y); // WARNING: index
            }
            else 
                _points.Add(pt);
            ApplyChanges();
        }

        public void Clear() {
            _points.Clear();
            _pointsView.Clear();
            d_points.Clear();
            i_points.Clear();
        }
    }
}