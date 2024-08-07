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

namespace LittleBlocks.Logging.SeriLog.Loggly;

public sealed class FluentLogglySinkBuilder : IControlLogLevel, ISinkBuilder, ISetCustomerToken, IConfigureLogBuffer
{
    private const int SslPort = 443;
    private readonly ISinkBuilderContext _sinkBuilderContext;
    private readonly Uri _serverUri;
    private bool _allowLogLevelToBeControlledRemotely;
    private string _bufferBaseFilename;
    private string _customerToken;

    public FluentLogglySinkBuilder(ISinkBuilderContext sinkBuilderContext, Uri serverUri)
    {
        _sinkBuilderContext =
            sinkBuilderContext ?? throw new ArgumentNullException(nameof(sinkBuilderContext));
        _serverUri = serverUri ?? throw new ArgumentNullException(nameof(serverUri));
    }

    public IControlLogLevel BufferLogsAt(string bufferBaseFilename)
    {
        if (string.IsNullOrWhiteSpace(bufferBaseFilename))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(bufferBaseFilename));
        _bufferBaseFilename = bufferBaseFilename;
        return this;
    }

    public ISinkBuilder EnableLogLevelControl()
    {
        _allowLogLevelToBeControlledRemotely = true;
        return this;
    }

    public LoggerConfiguration Build()
    {
        var config = LogglyConfig.Instance;
        config.CustomerToken = _customerToken;
        config.ApplicationName = $"MyApp-{_sinkBuilderContext.EnvironmentName}";
        config.IsEnabled = true;

        config.Transport.EndpointHostname = _serverUri.Host;
        config.Transport.EndpointPort = SslPort;
        config.Transport.LogTransport = LogTransport.Https;

        if (_allowLogLevelToBeControlledRemotely)
            return _sinkBuilderContext.LoggerConfiguration.WriteTo.Loggly(bufferBaseFilename: _bufferBaseFilename,
                controlLevelSwitch: LoggingLevelSwitchProvider.Instance);

        return _sinkBuilderContext.LoggerConfiguration.WriteTo.Loggly(bufferBaseFilename: _bufferBaseFilename);
    }

    public IConfigureLogBuffer WithCustomerToken(string customerToken)
    {
        if (string.IsNullOrWhiteSpace(customerToken))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(customerToken));
        _customerToken = customerToken;
        return this;
    }
}
