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

namespace LittleBlocks.RestEase;

public sealed class RequestContextBuilder<TContainer> where TContainer : class
{
    private RequestContextBuilder(TContainer container)
    {
        Container = container ?? throw new ArgumentNullException(nameof(container));
    }

    public TContainer Container { get; }

    public static RequestContextBuilder<TServicesContainer> For<TServicesContainer>(TServicesContainer container)
        where TServicesContainer : class
    {
        return new RequestContextBuilder<TServicesContainer>(container);
    }
}
