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

namespace LittleBlocks.Sample.WebAPI.Controllers;

[Route("api/[controller]")]
public class LogsController : Controller
{
    public LogsController(ILogger<LogsController> logger)
    {
        Log = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private ILogger<LogsController> Log { get; }

    [HttpGet]
    public IActionResult Get()
    {
        Log.LogTrace("Trace");
        Log.LogDebug("Debug");
        Log.LogInformation("Information");
        Log.LogWarning("Warning");
        Log.LogError("Error");
        Log.LogCritical("Critical");

        return Ok();
    }
}
