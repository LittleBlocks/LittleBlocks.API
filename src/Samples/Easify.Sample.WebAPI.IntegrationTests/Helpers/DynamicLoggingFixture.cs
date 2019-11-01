// This software is part of the Easify framework
// Copyright (C) 2019 Intermediate Capital Group
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

using System;
using System.Collections.Generic;
using System.IO;

namespace Easify.Sample.WebAPI.IntegrationTests.Helpers
{
    public sealed class DynamicLoggingFixture : IDisposable
    {
        public string LogDirectoryPath { get; private set; }
        public string LogFilePath { get; private set; }

        public void Dispose()
        {
            try
            {
                Directory.Delete(LogDirectoryPath, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public TestServerFixture<TStartup> CreateServer<TStartup>() where TStartup : class
        {
            var fixture = TestServerFixture<TStartup>.CreateWithLoggingEnabled();
            LogFilePath = fixture.LogFilePath;
            LogDirectoryPath = fixture.LogDirectoryPath;
            return fixture;
        }

        public IEnumerable<string> GetLogFileContents()
        {
            try
            {
                return File.ReadLines(LogFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}