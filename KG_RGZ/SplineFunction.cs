namespace KG_RGZ
{
    class SplineFunction {
        public float A = 0;
        public float B = 0;
        public float C = 0;
        public float D = 0;
        public float H = 0;
        public float X = 0;

        public SplineFunction()
        {
            
        }

        public SplineFunction(float a, float b, float c, float d, float h, float x) {
            A = a;
            B = b;
            C = c;
            D = d;
            H = h;
            X = x;
        }

        public float Func()
        {
            return Func(X);
        }

        public float Func(float s) {
            float ret = A;
            ret += B * (s - X);
            ret += C * (s - X) * (s - X);
            ret += D * (s - X) * (s - X) * (s - X);
            return ret;
        }
        public float D_Func(float s) //  жалкое подобие производной
        {
            float ret = 0;
            ret += B;
            ret += 2 * C * (s - X);
            ret += 3 * D * (s - X) * (s - X);
            return ret;
        }
        public float I_Func(float s) // жалкое подобие интеграла
        {
            float ret = A * (s - X);
            ret += B * (s - X) * (s - X) / 2;
            ret += C * (s - X) * (s - X) * (s - X) / 3;
            ret += D * (s - X) * (s - X) * (s - X) * (s - X) / 4;
            return ret;
            /*float ret = D * s * s * s * s / 4;
            ret += (C - 3 * D * X) * s * s * s / 3;
            ret += (3 * D * X * X - 2 * C * X + B) * s * s / 2;
            ret += (A - B * X + C * X * X - D * X * X * X) * s;
            return ret;*/
        }
    }
}
