namespace KG_RGZ
{
    class SplineFunction
    {
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

        public float Func(float s)
        {
            float res = A;
            res += B * (s - X);
            res += C * (s - X) * (s - X);
            res += D * (s - X) * (s - X) * (s - X);
            return res;
        }
        public float D_Func(float s) //  жалкое подобие производной
        {
            float res = 0;
            res += B;
            res += 2 * C * (s - X);
            res += 3 * D * (s - X) * (s - X);
            return res;
        }
        public float I_Func(float s) // жалкое подобие интеграла
        {
            float res = A * (s - X);
            res += B * (s - X) * (s - X) / 2;
            res += C * (s - X) * (s - X) * (s - X) / 3;
            res += D * (s - X) * (s - X) * (s - X) * (s - X) / 4;
            return res;
            /*float ret = D * s * s * s * s / 4;
            ret += (C - 3 * D * X) * s * s * s / 3;
            ret += (3 * D * X * X - 2 * C * X + B) * s * s / 2;
            ret += (A - B * X + C * X * X - D * X * X * X) * s;
            return ret;*/
        }
    }
}
