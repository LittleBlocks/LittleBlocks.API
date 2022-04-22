// This software is part of the Easify framework
// Copyright (C) 2019 Intermediate Capital Group
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

namespace Easify.ExceptionHandling.ErrorBuilder.Fluent;

public sealed class ErrorBuilderConfiguration<TException> : ISetErrorBuilder<TException>,
    IProvideErrorBuilder<TException> where TException : Exception
{
    private IErrorBuilder<TException> _builder = new DefaultErrorBuilder<TException>();

    public IErrorBuilder<TException> GetBuilder()
    {
        return _builder;
    }

    public IProvideErrorBuilder<TException> UseDefault()
    {
        _builder = new DefaultErrorBuilder<TException>();
        return this;
    }

    public IProvideErrorBuilder<TException> Use(IErrorBuilder<TException> builder)
    {
        _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        return this;
    }
}
