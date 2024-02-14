using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;

namespace gomoru.su.MMDBlendshapeGenerator
{
    internal static class ArrayUtils
    {
        public static ArraySizeChanger<T> UnsafeSetLength<T>(this T[] array, int length) => new(array, length);


        public readonly ref struct ArraySizeChanger<T>
        {
            private readonly T[] array;
            private readonly int previousLength;

            public ArraySizeChanger(T[] array, int size)
            {
                this.array = array;
                previousLength = array.Length;

                Unsafe.As<DummyArray>(array).Length = size;
            }

            public void Dispose()
            {
                Unsafe.As<DummyArray>(array).Length = previousLength;
            }

            private class DummyArray
            {
                public int Length;
            }
        }

        public static ArrayReleaser<T> CreateReleaser<T>(this T[] array) => new(array);

        public readonly ref struct ArrayReleaser<T>
        {
            private readonly T[] array;
            public ArrayReleaser(T[] array) => this.array = array;
            public void Dispose() => ArrayPool<T>.Shared.Return(array);
        }

        public static void Clear<T>(this T[] array) => array.AsSpan().Clear();
    }
}
