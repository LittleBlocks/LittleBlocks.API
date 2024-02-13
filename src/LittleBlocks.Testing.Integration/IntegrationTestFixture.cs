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

namespace LittleBlocks.Testing.Integration;

public class IntegrationTestFixture<TStartup> : FixtureBase where TStartup : class
{
    public TestServer GetServer(Action<TestServer> configure = null)
    {
        var app = CreateApplicationFactory();
        var testServer = app.Server;

        configure?.Invoke(testServer);
        return testServer;
    }

    public WebApplicationFactory<TStartup> GetApplicationFactory(Action<WebApplicationFactory<TStartup>> configure = null)
    {
        var app = CreateApplicationFactory();

        configure?.Invoke(app);
        return app;
    }

    public ClientAppPair<T, TStartup> CreateClientFromServer<T>(Action<IServiceProvider> setup = null) where T : IRestClient
    {
        var server = GetApplicationFactory(app =>
        {
            setup?.Invoke(app.Services);
        });

        var client = server.CreateClient<TStartup, T>();
        return new ClientAppPair<T, TStartup>(client, server);
    }
    
    

    protected virtual WebApplicationFactory<TStartup> CreateApplicationFactory()
    {
        return new IntegrationTestApplicationFactory<TStartup>();
    }
}
