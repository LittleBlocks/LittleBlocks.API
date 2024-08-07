// This software is part of the LittleBlocks framework
// Copyright (C) 2024 LittleBlocks
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

namespace LittleBlocks.ExceptionHandling.ErrorBuilder;

public sealed class DefaultErrorBuilder<TException> : IErrorBuilder<TException> where TException : Exception
{
    public Error Build(TException exception, IEnumerable<Error> internalErrors, bool includeSystemLevelExceptions)
    {
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        if (internalErrors == null) throw new ArgumentNullException(nameof(internalErrors));
        return new Error(exception.Message, exception.GetType().Name, internalErrors);
    }
}
