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

namespace LittleBlocks.AspNetCore.Dignostics;

public sealed class DiagnosticsMiddleware
{
    private const string JsonContentType = "application/json";
    private readonly IWebHostEnvironment _host;
    private readonly RequestDelegate _next;

    public DiagnosticsMiddleware(RequestDelegate next, IWebHostEnvironment host)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _host = host ?? throw new ArgumentNullException(nameof(host));
    }

    public async Task Invoke(HttpContext context)
    {
        var uri = new Uri(context.Request.GetDisplayUrl(), UriKind.RelativeOrAbsolute);
        if (uri.AbsolutePath.Equals("/diagnostics/status", StringComparison.OrdinalIgnoreCase))
        {
            var health = GenerateHealthInfo();
            context.Response.ContentType = JsonContentType;
            await context.Response.WriteAsync(health);

            return;
        }

        await _next.Invoke(context);
    }

    public string GenerateHealthInfo()
    {
        var version =
            Assembly.GetEntryAssembly()?
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

        var process = Process.GetCurrentProcess();
        var module = process.MainModule;
        var runtimeInfo = new
        {
            Pid = process.Id,
            Process = process.ProcessName,
            Release = module.FileName,
            Version = version,
            UpTime = $"{(DateTime.Now - process.StartTime).TotalSeconds:N2}s",
            Memory = $"{process.PrivateMemorySize64 / 1024:N2} MB",
            Cwd = _host.ContentRootPath
        };

        var hostInfo = new
        {
            Hostname = Dns.GetHostName(),
            OS = RuntimeInformation.OSDescription,
            Arch = RuntimeInformation.OSArchitecture
        };

        var healthInfo = new
        {
            Status = "Success",
            Service = new object(),
            Runtime = runtimeInfo,
            Host = hostInfo,
            Logs = new
            {
                LatestErrors = LogsStore.Instance.Errors.OrderByDescending(e => e.LoggedAt).ToList(),
                Messages = LogsStore.Instance.Logs.OrderByDescending(e => e.LoggedAt).ToList()
            }
        };
        return JsonConvert.SerializeObject(healthInfo, Formatting.Indented);
    }
}
