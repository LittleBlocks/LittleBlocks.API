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

namespace LittleBlocks.AspNetCore.Bootstrap;

public interface IHandleAdditionalException : ISetDetailsLevel
{
    IHandleAdditionalException AndHandle<TThirdPartyBaseException>()
        where TThirdPartyBaseException : Exception;

    IHandleAdditionalException AndHandle<TThirdPartyBaseException>(Func<TThirdPartyBaseException, bool> predicate)
        where TThirdPartyBaseException : Exception;

    IHandleAdditionalException AndHandle<TThirdPartyBaseException>(
        Func<ISetErrorBuilder<TThirdPartyBaseException>, IProvideErrorBuilder<TThirdPartyBaseException>>
            errorBuilderProvider)
        where TThirdPartyBaseException : Exception;

    IHandleAdditionalException AndHandle<TThirdPartyBaseException>(
        Func<ISetErrorBuilder<TThirdPartyBaseException>, IProvideErrorBuilder<TThirdPartyBaseException>>
            errorBuilderProvider, Func<TThirdPartyBaseException, bool> predicate)
        where TThirdPartyBaseException : Exception;
}
