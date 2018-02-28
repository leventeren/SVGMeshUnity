﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SVGMeshUnity.Internals
{
    public class WorkBuffer<T>
    {
        public WorkBuffer(int size = 32)
        {
            GrowSize = size;
            PrivateData = new T[size];
        }
        
        public T[] Data
        {
            get { return PrivateData; }
        }
        public int UsedSize
        {
            get { return PrivateUsedSize; }
        }

        public Func<T> NewForClass;

        private int GrowSize;
        private T[] PrivateData;
        private int PrivateUsedSize;

        private void Grow()
        {
            var newPrivateData = new T[PrivateData.Length + GrowSize];
            PrivateData.CopyTo(newPrivateData, 0);
            PrivateData = newPrivateData;
        }

        private void GrowIfNeeded()
        {
            if (PrivateData.Length == PrivateUsedSize)
            {
                Grow();
            }
        }

        public void Push(ref T val)
        {
            GrowIfNeeded();
            PrivateData[PrivateUsedSize] = val;
            ++PrivateUsedSize;
        }

        public T Push()
        {
            GrowIfNeeded();

            var val = PrivateData[PrivateUsedSize];

            if (val == null)
            {
                val = NewForClass();
                PrivateData[PrivateUsedSize] = val;
            }

            ++PrivateUsedSize;

            return val;
        }

        public T Pop()
        {
            var val = PrivateData[PrivateUsedSize - 1];
            --PrivateUsedSize;
            return val;
        }

        public T Insert(int index)
        {
            if (index == PrivateUsedSize)
            {
                return Push();
            }

            GrowIfNeeded();

            var val = PrivateData[PrivateUsedSize];

            for (var i = PrivateUsedSize - 1; i >= index; --i)
            {
                PrivateData[i + 1] = PrivateData[i];
            }

            if (val == null)
            {
                val = NewForClass();
            }

            PrivateData[index] = val;

            ++PrivateUsedSize;

            return val;
        }

        public void RemoveAt(int index)
        {
            var old = PrivateData[index];
            
            for (var i = index; i < PrivateUsedSize - 1; ++i)
            {
                PrivateData[i] = PrivateData[i + 1];
            }

            PrivateData[PrivateUsedSize - 1] = old;

            --PrivateUsedSize;
        }

        public void Sort(IComparer<T> c)
        {
            Array.Sort(PrivateData, 0, PrivateUsedSize, c);
        }

        public void RemoveLast(int n)
        {
            PrivateUsedSize -= n;
        }

        public void Clear()
        {
            PrivateUsedSize = 0;
        }

        public void Dump()
        {
            Debug.LogFormat("{0}{1}", PrivateUsedSize, PrivateData.Select(_ => string.Format("{0:x}",_ != null ? _.GetHashCode() : 0)).Aggregate("", (_, s) => _ + ", " + s));
        }
    }
}