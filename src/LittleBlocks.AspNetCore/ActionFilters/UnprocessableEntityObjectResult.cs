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

namespace LittleBlocks.AspNetCore.ActionFilters;

public sealed class UnprocessableEntityObjectResult : ObjectResult
{
    private const int UnprocessableEntity = 422;

    public UnprocessableEntityObjectResult(ModelStateDictionary modelState)
        : base(new SerializableError(modelState))
    {
        if (modelState == null)
            throw new ArgumentNullException(nameof(modelState));

        StatusCode = UnprocessableEntity;
    }
}
