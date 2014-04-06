using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NQuery.Binding
{
    internal sealed class BoundSortRelation : BoundRelation
    {
        private readonly bool _isDistinct;
        private readonly BoundRelation _input;
        private readonly ImmutableArray<BoundSortedValue> _sortedValues;

        public BoundSortRelation(bool isDistinct, BoundRelation input, IEnumerable<BoundSortedValue> sortedValues)
        {
            _input = input;
            _sortedValues = sortedValues.ToImmutableArray();
            _isDistinct = isDistinct;
        }

        public override BoundNodeKind Kind
        {
            get { return BoundNodeKind.SortRelation;}
        }

        public BoundRelation Input
        {
            get { return _input; }
        }

        public ImmutableArray<BoundSortedValue> SortedValues
        {
            get { return _sortedValues; }
        }

        public bool IsDistinct
        {
            get { return _isDistinct; }
        }

        public BoundSortRelation Update(bool isDistinct, BoundRelation input, IEnumerable<BoundSortedValue> sortedValues)
        {
            var newSortedValues = sortedValues.ToImmutableArray();

            if (isDistinct == _isDistinct && input == _input && newSortedValues == _sortedValues)
                return this;

            return new BoundSortRelation(isDistinct, input, newSortedValues);
        }

        public override IEnumerable<ValueSlot> GetDefinedValues()
        {
            return Enumerable.Empty<ValueSlot>();
        }

        public override IEnumerable<ValueSlot> GetOutputValues()
        {
            return _input.GetOutputValues();
        }
    }
}