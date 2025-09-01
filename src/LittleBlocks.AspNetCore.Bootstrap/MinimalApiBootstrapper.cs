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

/// <summary>
/// Bootstrapper for minimal APIs that provides the same configuration capabilities as the traditional AppBootstrapper
/// but without requiring a Startup class.
/// </summary>
public sealed class MinimalApiBootstrapper :
    IBootstrapApplication,
    IConfigureContainer,
    IAddExtraConfigSection,
    IHandleAdditionalException,
    ISetDetailsLevel,
    IExtendPipeline,
    IConfigureRequestCorrelation,
    IConfigureAuthentication,
    IConfigureHealthChecks,
    IConfigureApplicationBootstrapper
{
    private readonly IConfiguration _configuration;
    private readonly ConfigurationOptionBuilder _configurationOptionBuilder;
    private readonly GlobalErrorHandlerConfigurationBuilder _errorHandlerBuilder;
    private readonly List<Action<IServiceCollection, IConfiguration>> _pipelineExtenders =
        new List<Action<IServiceCollection, IConfiguration>>();

    private readonly IServiceCollection _services;
    private readonly IHealthChecksBuilder _healthChecksBuilder;
    private readonly AppInfo _appInfo;

    private readonly AuthOptions _authOptions;
    private Action<IServiceCollection, IConfiguration> _containerFactory;
    private Func<IExcludeRequests, IBuildOptions> _requestCorrelationExtender = cop => cop.EnforceCorrelation();

    public MinimalApiBootstrapper(
        IServiceCollection services,
        IConfiguration configuration)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _configurationOptionBuilder = new ConfigurationOptionBuilder(services, _configuration);
        _errorHandlerBuilder = new GlobalErrorHandlerConfigurationBuilder(services);

        _errorHandlerBuilder.UseStandardMessage();
        _appInfo = _configuration.GetApplicationInfo();
        _authOptions = _configuration.GetAuthOptions();

        _healthChecksBuilder = _services.AddHealthChecks();
    }

    public IAddExtraConfigSection AndSection<TSection>()
        where TSection : class, new()
    {
        _configurationOptionBuilder.And<TSection>();
        return this;
    }

    public IAddExtraConfigSection AndSection<TSection>(string section)
        where TSection : class, new()
    {
        _configurationOptionBuilder.And<TSection>(section);
        return this;
    }

    public IHandleAdditionalException HandleApplicationException<TApplicationBaseException>()
        where TApplicationBaseException : Exception
    {
        _errorHandlerBuilder.Handle<TApplicationBaseException>();
        return this;
    }

    public void Bootstrap()
    {
        _configurationOptionBuilder.Build();

        _services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        // Don't add IUrlHelper for minimal APIs as it requires MVC ActionContext
        _services.TryAddScoped<IArgumentsFormatter, ArgumentsFormatter>();
        _services.TryAddSingleton(_ => new ArgumentFormatterOptions());

        _services.AddDatabaseDeveloperPageExceptionFilter();
        _services.AddHttpRequestContext();
        _services.AddGlobalExceptionHandler(_ => _errorHandlerBuilder.UseDefault());
        _services.AddRequestCorrelation(b => _requestCorrelationExtender(b.ExcludeDefaultUrls()));
        _services.AddFeatureFlagging(_configuration);
        
        // For minimal APIs, we don't add the default MVC services automatically
        // Users can still add controllers if needed via AddControllers() on the builder
        _services.AddDefaultCorsPolicy();
        _services.AddAuthentication(_authOptions);
        _services.AddAuthorization(); // Required for minimal APIs
        
        // Add minimal API specific services for OpenAPI/Swagger
        _services.AddEndpointsApiExplorer();
        _services.AddSwaggerGen();

        _pipelineExtenders.ForEach(e => e(_services, _configuration));

        _containerFactory(_services, _configuration);
    }

    public IAddExtraConfigSection AddConfigSection<TSection>()
        where TSection : class, new()
    {
        _configurationOptionBuilder.AddSection<TSection>();
        return this;
    }

    public IAddExtraConfigSection AddConfigSection<TSection>(string section)
        where TSection : class, new()
    {
        _configurationOptionBuilder.AddSection<TSection>(section);
        return this;
    }

    public IBootstrapApplication UseContainer<TContainer>(ContainerFactory<TContainer> containerFactory)
        where TContainer : class
    {
        if (containerFactory == null) throw new ArgumentNullException(nameof(containerFactory));

        _containerFactory = containerFactory.Create;
        return this;
    }

    public IConfigureRequestCorrelation UseStandardMessage()
    {
        _errorHandlerBuilder.UseStandardMessage();
        return this;
    }

    public IConfigureRequestCorrelation UseUserErrors()
    {
        _errorHandlerBuilder.UseUserErrors();
        return this;
    }

    public IConfigureRequestCorrelation UseDetailedErrors()
    {
        _errorHandlerBuilder.UseDetailedErrors();
        return this;
    }

    public IConfigureAuthentication ConfigureCorrelation(
        Func<IExcludeRequests, IBuildOptions> optionsProvider)
    {
        _requestCorrelationExtender = optionsProvider ??
                                      throw new ArgumentNullException(nameof(optionsProvider));
        return this;
    }

    public IConfigureAuthentication ConfigureCorrelation(Func<IExcludeRequests, ICorrelateRequests> optionsProvider)
    {
        if (optionsProvider == null) throw new ArgumentNullException(nameof(optionsProvider));
        _requestCorrelationExtender = r => optionsProvider(r).EnforceCorrelation();
        return this;
    }

    public IExtendPipeline Extend(Action<IServiceCollection, IConfiguration> pipelineExtender)
    {
        if (pipelineExtender == null) throw new ArgumentNullException(nameof(pipelineExtender));
        _pipelineExtenders.Add(pipelineExtender);
        return this;
    }

    public IConfigureHealthChecks ConfigureAuthentication(Action<ISetAuthenticationMode> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));
        configure(_authOptions);
        return this;
    }

    public IExtendPipeline ConfigureHealthChecks(Action<IHealthChecksBuilder> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        configure(_healthChecksBuilder);

        return this;
    }

    public IHandleAdditionalException AndHandle<TThirdPartyBaseException>()
        where TThirdPartyBaseException : Exception
    {
        _errorHandlerBuilder.AndHandle<TThirdPartyBaseException>();
        return this;
    }

    public IHandleAdditionalException AndHandle<TThirdPartyBaseException>(
        Func<TThirdPartyBaseException, bool> predicate) where TThirdPartyBaseException : Exception
    {
        _errorHandlerBuilder.AndHandle(predicate);
        return this;
    }

    public IHandleAdditionalException AndHandle<TThirdPartyBaseException>(
        Func<ISetErrorBuilder<TThirdPartyBaseException>, IProvideErrorBuilder<TThirdPartyBaseException>>
            errorBuilderProvider) where TThirdPartyBaseException : Exception
    {
        _errorHandlerBuilder.AndHandle(errorBuilderProvider);
        return this;
    }

    public IHandleAdditionalException AndHandle<TThirdPartyBaseException>(
        Func<ISetErrorBuilder<TThirdPartyBaseException>, IProvideErrorBuilder<TThirdPartyBaseException>>
            errorBuilderProvider, Func<TThirdPartyBaseException, bool> predicate)
        where TThirdPartyBaseException : Exception
    {
        _errorHandlerBuilder.AndHandle(errorBuilderProvider, predicate);
        return this;
    }
}