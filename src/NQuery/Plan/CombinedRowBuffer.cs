﻿using System;

namespace NQuery.Plan
{
    internal sealed class CombinedRowBuffer : RowBuffer
    {
        private readonly RowBuffer _left;
        private readonly RowBuffer _right;

        public CombinedRowBuffer(RowBuffer left, RowBuffer right)
        {
            _left = left;
            _right = right;
        }

        public override int Count
        {
            get { return _left.Count + _right.Count; }
        }

        public override object this[int index]
        {
            get
            {
                return index < _left.Count
                           ? _left[index]
                           : _right[index - _left.Count];
            }
        }
    }
}