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

namespace LittleBlocks.Sample.WebAPI.Core.Mappings;

public sealed class AssetConverter : ITypeConverter<AssetEntity, AssetDo>
{
    private readonly IRateProvider _rateProvider;

    public AssetConverter(IRateProvider rateProvider)
    {
        _rateProvider = rateProvider ?? throw new ArgumentNullException(nameof(rateProvider));
    }

    public AssetDo Convert(AssetEntity source, AssetDo destination, ResolutionContext context)
    {
        if (source == null) return null;

        return new AssetDo
        {
            Id = source.Id,
            Rating = _rateProvider.GetRating(source.Id)
        };
    }
}
