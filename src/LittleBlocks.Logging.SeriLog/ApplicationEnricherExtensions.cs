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

namespace LittleBlocks.Logging.SeriLog;

public static class ApplicationEnricherExtensions
{
    public const string ApplicationPropertyName = "Application";
    public const string EnvironmentPropertyName = "Environment";

    public static LoggerConfiguration WithApplicationName(this LoggerEnrichmentConfiguration config,
        string applicationName)
    {
        return config.WithProperty(ApplicationPropertyName, applicationName);
    }

    public static LoggerConfiguration WithEnvironmentName(this LoggerEnrichmentConfiguration config,
        string environmentName)
    {
        return config.WithProperty(EnvironmentPropertyName, environmentName);
    }
}
