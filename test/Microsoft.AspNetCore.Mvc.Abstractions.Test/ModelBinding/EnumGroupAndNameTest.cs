using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Abstractions.Test.ModelBinding
{
    public class EnumGroupAndNameTest
    {
        [Fact]
        public void EnumGroupAndName_NameWithNoResourceTypeAndNoIStringLocalizer()
        {
            // Arrange & Act
            var enumGroupAndName = GetEnumGroupAndName(EnumWithLocalizedDisplayNames.Two, iStringLocalizer: false);

            // Assert
            using (new CultureReplacer("en-US", "en-US"))
            {
                Assert.Equal("Loc_Two_Name", enumGroupAndName.Name);
            }

            using (new CultureReplacer("fr-FR", "fr-FR"))
            {
                Assert.Equal("Loc_Two_Name", enumGroupAndName.Name);
            }
        }

        [Fact]
        public void EnumGroupAndName_NameWithNoResourceTypeAndIStringLocalizer()
        {
            // Arrange & Act
            var enumGroupAndName = GetEnumGroupAndName(EnumWithLocalizedDisplayNames.Two, iStringLocalizer: true);

            // Assert
            using (new CultureReplacer("en-US", "en-US"))
            {
                Assert.Equal("Loc_Two_Name en-US", enumGroupAndName.Name);
            }

            using (new CultureReplacer("fr-FR", "fr-FR"))
            {
                Assert.Equal("Loc_Two_Name fr-FR", enumGroupAndName.Name);
            }
        }

        [Fact]
        public void EnumGroupAndName_NameWithResourceTypeAndIStringLocalizer()
        {
            // Arrange & Act
            var enumGroupAndName = GetEnumGroupAndName(EnumWithLocalizedDisplayNames.Three, iStringLocalizer: true);

            // Assert
            using (new CultureReplacer("en-US", "en-US"))
            {
                Assert.Equal("type three name en-US", enumGroupAndName.Name);
            }

            using (new CultureReplacer("fr-FR", "fr-FR"))
            {
                Assert.Equal("type three name fr-FR", enumGroupAndName.Name);
            }
        }

        [Fact]
        public void EnumGroupAndName_NameWithResourceTypeAndNoIStringLocalizer()
        {
            // Arrange & Act
            var enumGroupAndName = GetEnumGroupAndName(EnumWithLocalizedDisplayNames.Three, iStringLocalizer: false);

            // Assert
            using (new CultureReplacer("en-US", "en-US"))
            {
                Assert.Equal("type three name en-US", enumGroupAndName.Name);
            }

            using (new CultureReplacer("fr-FR", "fr-FR"))
            {
                Assert.Equal("type three name fr-FR", enumGroupAndName.Name);
            }
        }

        private EnumGroupAndName GetEnumGroupAndName(EnumWithLocalizedDisplayNames enumValue, bool iStringLocalizer)
        {
            var field = typeof(EnumWithLocalizedDisplayNames).GetField(enumValue.ToString());
            var stringLocalizer = iStringLocalizer ? GetStringLocalizer() : null;
            return new EnumGroupAndName("", stringLocalizer, field);
        }

        private IStringLocalizer GetStringLocalizer()
        {
            var stringLocalizer = new Mock<IStringLocalizer>(MockBehavior.Strict);
            stringLocalizer
                .Setup(loc => loc[It.IsAny<string>()])
                .Returns<string>((k =>
                {
                    return new LocalizedString(k, $"{k} {CultureInfo.CurrentCulture}");
                }));

            var stringLocalizerFactory = new Mock<IStringLocalizerFactory>(MockBehavior.Strict);
            stringLocalizerFactory
                .Setup(factory => factory.Create(typeof(EnumWithLocalizedDisplayNames)))
                .Returns(stringLocalizer.Object);

            return stringLocalizer.Object;
        }

        private enum EnumWithLocalizedDisplayNames
        {
            [Display(Name = "Loc_Two_Name")]
            Two = 2,
            [Display(
                Name = "Type_Three_Name",
                ResourceType = typeof(TestResources))]
            Three = 3
        }
    }
}
