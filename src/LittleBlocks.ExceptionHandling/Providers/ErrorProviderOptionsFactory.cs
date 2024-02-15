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

namespace LittleBlocks.ExceptionHandling.Providers;

public sealed class ErrorProviderOptionsFactory : IErrorProviderOptions
{
    private ErrorProviderOptionsFactory(IErrorResponseProviderOptions options, bool includeSystemLevelExceptions)
    {
        GenericError = options.GenericError;
        ExceptionsToHandle = options.RulesForExceptionHandling;
        IncludeSystemLevelExceptions = includeSystemLevelExceptions;
    }

    public Error GenericError { get; }
    public IReadOnlyList<IExceptionRule> ExceptionsToHandle { get; }
    public bool IncludeSystemLevelExceptions { get; }

    public static IErrorProviderOptions IncludeDetailedErrors(IErrorResponseProviderOptions options)
    {
        return new ErrorProviderOptionsFactory(options, true);
    }

    public static IErrorProviderOptions ExcludeDetailedErrors(IErrorResponseProviderOptions options)
    {
        return new ErrorProviderOptionsFactory(options, false);
    }
}
