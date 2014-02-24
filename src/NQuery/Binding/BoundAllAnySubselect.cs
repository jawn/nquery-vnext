using System;

namespace NQuery.Binding
{
    internal sealed class BoundAllAnySubselect : BoundExpression
    {
        private readonly BoundExpression _left;
        private readonly BoundQueryRelation _query;
        private readonly OverloadResolutionResult<BinaryOperatorSignature> _result;

        public BoundAllAnySubselect(BoundExpression left, BoundQueryRelation query, OverloadResolutionResult<BinaryOperatorSignature> result)
        {
            _left = left;
            _query = query;
            _result = result;
        }

        public override BoundNodeKind Kind
        {
            get { return BoundNodeKind.AllAnySubselect; }
        }

        public BoundExpression Left
        {
            get { return _left; }
        }

        public BoundQueryRelation Query
        {
            get { return _query; }
        }

        public OverloadResolutionResult<BinaryOperatorSignature> Result
        {
            get { return _result; }
        }

        public override Type Type
        {
            get
            {
                return _result.Selected == null
                           ? TypeFacts.Unknown
                           : _result.Selected.Signature.ReturnType;
            }
        }
    }
}