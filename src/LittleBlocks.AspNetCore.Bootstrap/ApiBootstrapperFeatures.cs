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

namespace LittleBlocks.AspNetCore.Bootstrap;

[Flags]
public enum ApiBootstrapperFeatures
{
    None = 0,
    Configuration = 1 << 0,
    RequestCorrelation = 1 << 1,
    FeatureFlags = 1 << 2,
    Cors = 1 << 3,
    Authentication = 1 << 4,
    OpenApi = 1 << 5,
    HealthChecks = 1 << 6,
    ExceptionHandling = 1 << 7,
    Diagnostics = 1 << 8,
    StaticFiles = 1 << 9,
    HttpsRedirection = 1 << 10,
    Controllers = 1 << 11,
    StartPage = 1 << 12,
    All = Configuration | RequestCorrelation | FeatureFlags | Cors | Authentication | OpenApi | HealthChecks |
          ExceptionHandling | Diagnostics | StaticFiles | HttpsRedirection | Controllers | StartPage,
    MinimalDefaults = All & ~Controllers,
    MvcDefaults = All
}
