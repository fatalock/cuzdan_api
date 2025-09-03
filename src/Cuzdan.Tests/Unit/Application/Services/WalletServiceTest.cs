// Cuzdan.Tests/Unit/Application/Validators/CreateWalletDtoValidatorTests.cs (YENİ DOSYA)
using Cuzdan.Application.DTOs;
using Cuzdan.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Cuzdan.Tests.Unit.Application.Validators;

public class CreateWalletDtoValidatorTests
{
    private readonly CreateWalletDtoValidator _validator;

    public CreateWalletDtoValidatorTests()
    {
        _validator = new CreateWalletDtoValidator();
    }

    [Fact]
    public void Should_have_error_when_WalletName_is_null()
    {
        // Arrange
        var dto = new CreateWalletDto { WalletName = null, Currency = Domain.Enums.CurrencyType.TRY };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WalletName);
    }

    [Fact]
    public void Should_not_have_error_when_WalletName_is_specified()
    {
        // Arrange
        var dto = new CreateWalletDto { WalletName = "Maaş Hesabım", Currency = Domain.Enums.CurrencyType.TRY };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.WalletName);
    }
}