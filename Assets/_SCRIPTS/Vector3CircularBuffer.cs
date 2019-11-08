using UnityEngine;

namespace Dream
{
    public class Vector3CircularBuffer
    {
        Vector3[] circularBuffer;
        int nextIndex = 0;
        int numElements = 0;

        public Vector3CircularBuffer(int capacity)
        {
            circularBuffer = new Vector3[capacity];
        }

        public void Add(Vector3 toAdd)
        {
            circularBuffer[nextIndex] = toAdd;
            if (++nextIndex >= circularBuffer.Length)
            {
                nextIndex = 0;
            }
            numElements = Mathf.Min(numElements + 1, circularBuffer.Length);
        }

        public float AverageDistanceFromOldestPoint()
        {
            if (numElements < 2)
            {
                return 0;
            }

            float totalDistance = 0;
            if (numElements == circularBuffer.Length)
            {
                // Buffer is full.
                int oldestIndex = nextIndex - 1 < 0 ? circularBuffer.Length - 1 : nextIndex - 1;
                for (int i = 0; i < circularBuffer.Length; ++i)
                {
                    if (i != oldestIndex)
                    {
                        totalDistance += Vector3.Distance(circularBuffer[oldestIndex], circularBuffer[i]);
                    }
                }
            }
            else
            {
                // Buffer is not full.
                for (int i = 1; i < circularBuffer.Length; ++i)
                {
                    totalDistance += Vector3.Distance(circularBuffer[0], circularBuffer[i]);
                }
            }

            return totalDistance / (circularBuffer.Length - 1);
        }
    }
}
