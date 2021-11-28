using System;
using System.Diagnostics;
using System.Collections.Generic;
using PacManLogic.GameField;

namespace PacManLogic.Helpers
{
    public class Matrix<T> : IEnumerable<T>
    {
        T[,] matrix;
        public int M { get; private set; }
        public int N { get; private set; }
        
        public T this[int m, int n] 
        {
            get { return matrix[m, n]; }
            set { matrix[m, n] = value; }
        }

        public T this[Point p]
        {
            get { return matrix[p.X, p.Y]; }
            set { matrix[p.X, p.Y] = value;}
        }

        public Matrix(int m, int n)
        {
            matrix = new T[m,n];
            M = m;
            N = n;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    yield return matrix[i, j];
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
