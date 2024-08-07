﻿// This software is part of the LittleBlocks framework
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

namespace LittleBlocks.Resilience;

public sealed class PolicyCircuitBreakOptions
{
    public int NumberOfExceptionsBefore { get; set; } = 2;
    public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromMinutes(1);
    public Action<Exception, TimeSpan> OnBreak { get; set; } = (_, _) => { };
    public Action OnReset { get; set; } = () => { };
}
