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

namespace Easify.Logging.UnitTests;

public class ArgumentsFormatterTests
{
    public ArgumentsFormatterTests()
    {
        Log = Substitute.For<ILogger<ArgumentsFormatter>>();
        Sut = new ArgumentsFormatter(Log, new ArgumentFormatterOptions
        {
            Formatting = Formatting.Indented
        });
    }

    private ArgumentsFormatter Sut { get; }

    private ILogger<ArgumentsFormatter> Log { get; }

    [Theory]
    [InlineData(1, "1")]
    [InlineData("value", "value")]
    public void ShouldReturnGivenIntegerValue(object value, string expectedValue)
    {
        // Arrange
        // Act
        // Assert
        Assert.Equal(expectedValue, Sut.FormatArgument(value));
    }

    [Fact]
    public void ShouldReturnEmptyStringIfArgumentIsNull()
    {
        // Arrange
        // Act
        var result = Sut.FormatArgument(null);
        // Assert
        Assert.Equal("[NULL]", result);
    }

    [Fact]
    public void ShouldReturnFormattedJsonForGivenListOfArguments()
    {
        // Arrange
        var person = new Person
        {
            Name = "Algimantas",
            Age = 60,
            Sex = Sex.Male,
            Birthday = new DateTime(1965, 01, 01),
            Address = new Address
            {
                City = "London",
                Street = "Chatham st."
            }
        };
        var value = "Valuesasas";

        var sb = new StringBuilder();
        sb.AppendLine("[");
        sb.AppendLine("  \"Valuesasas\",");
        sb.AppendLine("  {");
        sb.AppendLine("    \"Name\": \"Algimantas\",");
        sb.AppendLine("    \"Age\": 60,");
        sb.AppendLine("    \"Sex\": \"Male\",");
        sb.AppendLine("    \"Birthday\": \"1965-01-01T00:00:00\",");
        sb.AppendLine("    \"Address\": {");
        sb.AppendLine("      \"City\": \"London\",");
        sb.AppendLine("      \"Street\": \"Chatham st.\"");
        sb.AppendLine("    }");
        sb.AppendLine("  }");
        sb.AppendLine("]");

        var expectedValue = sb.ToString().TrimEnd();

        // Act
        var result = Sut.FormatArgument(new List<object>
        {
            value,
            person
        });
        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void ShouldReturnFormattedJsonForGivenPerson()
    {
        // Arrange
        var person = new Person
        {
            Name = "Algimantas",
            Age = 60,
            Sex = Sex.Male,
            Birthday = new DateTime(1965, 01, 01),
            Address = new Address
            {
                City = "London",
                Street = "Chatham st."
            }
        };
        var expectedValue = JsonConvert.SerializeObject(person, Formatting.Indented, new StringEnumConverter());

        // Act
        // Assert
        Assert.Equal(expectedValue, Sut.FormatArgument(person));
    }

    [Fact]
    public void ShouldReturnMultipleArgumentsFormattedWithoutCommaAtTheEnd()
    {
        // Arrange
        var arguments = new object[2]
        {
            "value1",
            "value2"
        };

        // Act
        var result = Sut.FormatArguments(arguments);

        // Assert
        Assert.Equal("value1, value2.", result);
    }
}
