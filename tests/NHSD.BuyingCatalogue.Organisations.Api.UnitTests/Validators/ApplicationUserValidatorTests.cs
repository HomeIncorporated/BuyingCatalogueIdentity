﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.Validators;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Validators
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class ApplicationUserValidatorTests
    {
        private const string FiftyFiveCharacterString = "0123456789_0123456789_0123456789_0123456789_0123456789_";

        [Test]
        public async Task ValidateAsync_ValidApplicationUser_ReturnsSuccess()
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = ApplicationUserBuilder
                .Create()
                .BuildBuyer();

            var actual = await sut.ValidateAsync(user);

            actual.Should().Be(Result.Success());
        }

        [TestCase("", "FirstNameRequired")]
        [TestCase("  ", "FirstNameRequired")]
        [TestCase(FiftyFiveCharacterString, "FirstNameTooLong")]
        public async Task ValidateAsync_WithFirstName_ReturnsFailure(string input, params string[] errorMessageIds)
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = ApplicationUserBuilder
                .Create()
                .WithFirstName(input)
                .BuildBuyer();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorMessage(id, nameof(ApplicationUser.FirstName))));
            actual.Should().Be(expected);
        }

        [TestCase("", "LastNameRequired")]
        [TestCase("  ", "LastNameRequired")]
        [TestCase(FiftyFiveCharacterString, "LastNameTooLong")]
        public async Task ValidateAsync_WithLastName_ReturnsFailure(string input, params string[] errorMessageIds)
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = ApplicationUserBuilder
                .Create()
                .WithLastName(input)
                .BuildBuyer();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorMessage(id, nameof(ApplicationUser.LastName))));
            actual.Should().Be(expected);
        }

        [TestCase("", "PhoneNumberRequired")]
        [TestCase("  ", "PhoneNumberRequired")]
        public async Task ValidateAsync_WithPhoneNumber_ReturnsFailure(string input, params string[] errorMessageIds)
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = ApplicationUserBuilder
                .Create()
                .WithPhoneNumber(input)
                .BuildBuyer();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorMessage(id, nameof(ApplicationUser.PhoneNumber))));
            actual.Should().Be(expected);
        }

        [TestCaseSource(typeof(TestContextTestCaseData), nameof(TestContextTestCaseData.InvalidEmailTestCases))]
        public async Task ValidateAsync_WithEmailAddress_ReturnsFailure(string input, string[] errorMessageIds)
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = ApplicationUserBuilder
                .Create()
                .WithEmailAddress(input)
                .BuildBuyer();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorMessage(id, nameof(ApplicationUser.Email))));
            actual.Should().Be(expected);
        }

        [Test]
        public async Task ValidateAsync_DuplicateEmailAddress_ReturnsFailure()
        {
            const string duplicateEmailAddress = "duplicate@email.com";

            var context = ApplicationUserValidatorTestContext.Setup();
            context.ApplicationUserByEmail = ApplicationUserBuilder
                .Create()
                .WithEmailAddress(duplicateEmailAddress)
                .BuildBuyer();

            var sut = context.ApplicationUserValidator;

            var user = ApplicationUserBuilder
                .Create()
                .WithEmailAddress(duplicateEmailAddress)
                .BuildBuyer();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(new List<ErrorMessage> { new ErrorMessage("EmailAlreadyExists", nameof(ApplicationUser.Email)) });
            actual.Should().Be(expected);
        }

        [Test]
        public void Constructor_NullUserRepository_ThrowsException()
        {
            static void Test()
            {
                _ = new ApplicationUserValidator(null);
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void ValidateAsync_NullApplicationUser_ThrowsException()
        {
            static async Task Test()
            {
                var context = ApplicationUserValidatorTestContext.Setup();
                var sut = context.ApplicationUserValidator;
                
                await sut.ValidateAsync(null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(Test);
        }
    }

    internal class TestContextTestCaseData
    {
        internal static IEnumerable<TestCaseData> InvalidEmailTestCases
        {
            get
            {
                yield return new TestCaseData("", new[] { "EmailRequired" });
                yield return new TestCaseData("  ", new[] { "EmailRequired" });
                yield return new TestCaseData($"a@{new string('b', 255)}", new[] { "EmailTooLong" });
                yield return new TestCaseData("test", new[] { "EmailInvalidFormat" });
                yield return new TestCaseData("test@", new[] { "EmailInvalidFormat" });
                yield return new TestCaseData("@test", new[] { "EmailInvalidFormat" });
                yield return new TestCaseData("@", new[] { "EmailInvalidFormat" });
            }
        }
    }

    internal sealed class ApplicationUserValidatorTestContext
    {
        private ApplicationUserValidatorTestContext()
        {
            UsersRepositoryMock = new Mock<IUsersRepository>();
            UsersRepositoryMock.Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => ApplicationUserByEmail);

            ApplicationUserValidator = new ApplicationUserValidator(UsersRepositoryMock.Object);
        }

        internal Mock<IUsersRepository> UsersRepositoryMock { get; set; }

        internal ApplicationUserValidator ApplicationUserValidator { get; }

        internal ApplicationUser ApplicationUserByEmail { get; set; }

        public static ApplicationUserValidatorTestContext Setup()
        {
            return new ApplicationUserValidatorTestContext();
        }
    }
}