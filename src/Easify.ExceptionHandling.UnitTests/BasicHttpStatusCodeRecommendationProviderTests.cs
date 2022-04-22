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

namespace Easify.ExceptionHandling.UnitTests;

public sealed class BasicHttpStatusCodeRecommendationProviderTests
{
    [Fact]
    public void GivenExceptionWhenItsDerivativeOfAppExceptionThenHttpCodeShouldBe_400()
    {
        // Arrange
        var optionsMock = Substitute.For<IErrorResponseProviderOptions>();
        var sut = new DefaultHttpStatusCodeProvider(optionsMock);

        optionsMock.RulesForExceptionHandling.Returns(new List<IExceptionRule>
        {
            new ExceptionRuleForErrorProvider<ApplicationExceptionBase>(),
            new ExceptionRuleForErrorProvider<ThirdPartyFailureException>()
        });

        // Act
        var result = sut.GetHttpStatusCode(new OurApplicationException());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result);
    }

    [Fact]
    public void GivenExceptionWhenItsDerivativeOfThirdPartyExceptionThenHttpCodeShouldBe_400()
    {
        // Arrange
        var optionsMock = Substitute.For<IErrorResponseProviderOptions>();
        var sut = new DefaultHttpStatusCodeProvider(optionsMock);

        optionsMock.RulesForExceptionHandling.Returns(new List<IExceptionRule>
        {
            new ExceptionRuleForErrorProvider<ApplicationExceptionBase>(),
            new ExceptionRuleForErrorProvider<ThirdPartyFailureException>()
        });

        // Act
        var result = sut.GetHttpStatusCode(new ThirdPartyFailureException());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result);
    }

    [Fact]
    public void GivenExceptionWhenItsSystemExceptionThenHttpCodeShouldBe_500()
    {
        // Arrange
        var optionsMock = Substitute.For<IErrorResponseProviderOptions>();
        var sut = new DefaultHttpStatusCodeProvider(optionsMock);

        optionsMock.RulesForExceptionHandling.Returns(new List<IExceptionRule>
        {
            new ExceptionRuleForErrorProvider<ApplicationExceptionBase>(),
            new ExceptionRuleForErrorProvider<ThirdPartyFailureException>()
        });

        // Act
        var result = sut.GetHttpStatusCode(new Exception());

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result);
    }
}
