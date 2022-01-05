using UnityEngine;
namespace PlaneEngine
{
    public struct Matrix2x2
    {
        public Vector2 Col1;
        public Vector2 Col2;

        public Matrix2x2(Vector2 c1, Vector2 c2)
        {
            Col1 = c1;
            Col2 = c2;
        }

        public Matrix2x2(float a11, float a12, float a21, float a22)
        {
            Col1.x = a11; Col1.y = a21;
            Col2.x = a12; Col2.y = a22;
        }

        public Matrix2x2(float angle)
        {
            float c = Mathf.Cos(angle);
            float s = Mathf.Sin(angle);
            Col1.x = c; Col2.x = -s;
            Col1.y = s; Col2.y = c;
        }

        public void Set(Vector2 c1, Vector2 c2)
        {
            Col1 = c1;
            Col2 = c2;
        }

        public void Set(float angle)
        {
            float c = Mathf.Cos(angle);
            float s = Mathf.Sin(angle);
            Col1.x = c; Col2.x = -s;
            Col1.y = s; Col2.y = c;
        }

        public float GetAngle()
        {
            return Mathf.Atan2(Col1.y, Col1.x);
        }
        public Vector2 Multiply(Vector2 vector)
        {
            return new Vector2(Col1.x * vector.x + Col2.x * vector.y, Col1.y * vector.x + Col2.y * vector.y);
        }

        public Matrix2x2 GetInverse()
        {
            float a = Col1.x, b = Col2.x, c = Col1.y, d = Col2.y;
            Matrix2x2 B = new Matrix2x2();
            float det = a * d - b * c;
            Debug.Assert(det != 0.0f);
            det = 1.0f / det;
            B.Col1.x = det * d; B.Col2.x = -det * b;
            B.Col1.y = -det * c; B.Col2.y = det * a;
            return B;
        }
        public static Matrix2x2 Identity { get { return new Matrix2x2(1, 0, 0, 1); } }

        public static Matrix2x2 operator +(Matrix2x2 A, Matrix2x2 B)
        {
            Matrix2x2 C = new Matrix2x2();
            C.Set(A.Col1 + B.Col1, A.Col2 + B.Col2);
            return C;
        }
    }

}
