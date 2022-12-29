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

public static class ApiExceptionExtensions
{
    private const int UnprocessableEntity = 422;

    public static bool ClientError(this ApiException f)
    {
        return f.StatusCode == HttpStatusCode.BadRequest ||
               f.StatusCode == HttpStatusCode.Unauthorized ||
               f.StatusCode == HttpStatusCode.Forbidden ||
               f.StatusCode == HttpStatusCode.NotFound ||
               f.StatusCode == HttpStatusCode.MethodNotAllowed ||
               f.StatusCode == HttpStatusCode.NotAcceptable ||
               f.StatusCode == HttpStatusCode.RequestTimeout ||
               f.StatusCode == HttpStatusCode.Conflict ||
               f.StatusCode == HttpStatusCode.Gone ||
               f.StatusCode == HttpStatusCode.UnsupportedMediaType ||
               f.StatusCode == HttpStatusCode.ExpectationFailed ||
               f.StatusCode == HttpStatusCode.ProxyAuthenticationRequired ||
               (int) f.StatusCode == UnprocessableEntity;
    }
}
