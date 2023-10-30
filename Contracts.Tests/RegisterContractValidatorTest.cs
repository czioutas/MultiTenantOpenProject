using System;
using Bogus;
using FluentValidation.TestHelper;
using MultiTenantOpenProject.Contracts.Account;
using MultiTenantOpenProject.Contracts.Account.Validators;

namespace MultiTenantOpenProject.Contracts.Tests
{
    [TestClass]
    public class RegisterContractValidatorTest
    {
        private static string validPassword = "ZUM#gC$QgUyyUJQkYzR3";
        private static string validEmail = "chris@gmail.com";
        private RegisterContractValidator? validator;
        private RegisterContract? registerContract;

        [TestInitialize]
        public void TestInitialize()
        {
            validator = new RegisterContractValidator();
        }

        [TestMethod]
        [DataRow("", true)]
        [DataRow(null, true)]
        [DataRow("potato", true)]
        [DataRow("chris@gmail.com", false)]
        public void Validate_Email(string email, bool shouldHaveErrors)
        {
            registerContract = new RegisterContract(validPassword, email);

            var result = validator.TestValidate(registerContract);
            if (shouldHaveErrors)
            {
                result.ShouldHaveValidationErrorFor(x => x.Email);
            }
            else
            {
                result.ShouldNotHaveValidationErrorFor(x => x.Email);
            }
        }

        [TestMethod]
        [DataRow("", true)]
        [DataRow(null, true)]
        [DataRow("1", true)]
        [DataRow("x", true)] // we reserve x as a string that we translate to 500 characters
        [DataRow("ZUM#gC$QgUyyUJQkYzR3", false)]
        public void Validate_Password(string password, bool shouldHaveErrors)
        {
            if (password != null && password == "x")
            {
                password = new Faker().Internet.Password(500);
            }

            registerContract = new RegisterContract(password, validEmail);

            var result = validator.TestValidate(registerContract);
            if (shouldHaveErrors)
            {
                result.ShouldHaveValidationErrorFor(x => x.Password);
            }
            else
            {
                result.ShouldNotHaveValidationErrorFor(x => x.Password);
            }
        }

        [TestMethod]
        [DataRow("", "", true)]
        public void Validate_Password_And_Email(string password, string email, bool shouldHaveErrors)
        {
            registerContract = new RegisterContract(password, email);

            var result = validator.TestValidate(registerContract);
            if (shouldHaveErrors)
            {
                result.ShouldHaveValidationErrorFor(x => x.Password);
                result.ShouldHaveValidationErrorFor(x => x.Email);
            }
            else
            {
                result.ShouldNotHaveValidationErrorFor(x => x.Password);
                result.ShouldHaveValidationErrorFor(x => x.Email);
            }
        }

        // [TestMethod]
        // public void Validate_Password_Length_Is_Min()
        // {
        //     registerContract = new RegisterContract(fullyComplexPassword, "email");

        //     var result = validator.TestValidate(registerContract);
        //     result.ShouldHaveValidationErrorFor(x => x.Password);
        // }
    }
}
