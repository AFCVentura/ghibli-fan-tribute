using Bunit;
using GhibliTribute.Components;

namespace GhibliTribute.Tests.Components
{
    public class FieldErrorTests : BunitContext
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithoutMessage_RendersNothing(string? message)
        {
            var component = Render<FieldError>(parameters => parameters.Add(p => p.Message, message));

            Assert.Empty(component.Markup.Trim());
        }

        [Fact]
        public void WithMessage_RendersIt()
        {
            var component = Render<FieldError>(parameters => parameters.Add(p => p.Message, "Required field"));

            Assert.Equal("Required field", component.Find("p").TextContent);
        }
    }
}
