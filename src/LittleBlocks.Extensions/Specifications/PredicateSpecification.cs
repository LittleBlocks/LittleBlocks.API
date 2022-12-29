// This software is part of the LittleBlocks framework
// Copyright (C) 2022 LittleBlocks
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace LittleBlocks.Extensions.Specifications;

public sealed class PredicateSpecification<T> : Specification<T>
{
    private readonly Expression<Func<T, bool>> _predicate;

    public PredicateSpecification(Expression<Func<T, bool>> predicate)
    {
        _predicate = predicate;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        return _predicate;
    }
}
