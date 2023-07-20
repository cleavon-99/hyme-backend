using FluentAssertions;
using FluentAssertions.Equivalency.Steps;
using NetArchTest.Rules;
using System.Reflection;

namespace Architecture.Tests
{
    public class ArchitectureTests
    {
        private const string DomainNamespace = "Hyme.Domain";
        private const string ApplicationNamespace = "Hyme.Application";
        private const string InfrastructureNamespace = "Hyme.Infrastructure";
        private const string APINamespace = "Hyme.API";
        private const string SharedNamesppace = "Hyme.Shared";

        [Fact]
        public void Domain_ShouldNotHave_DependencyOnOtherProjects_ExceptShared()
        {

            //Arrange
            Assembly? assembly = Assembly.GetAssembly(typeof(Hyme.Domain.AssemblyReference));

            var otherProjects = new[]
            {
                ApplicationNamespace,
                InfrastructureNamespace,
                APINamespace,
                SharedNamesppace
            };

            //Act
            TestResult result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAll(otherProjects)
                .GetResult();

            //Assert
            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_ShouldNotHave_DependencyOnOtherProjects_ExceptDomainAndShared()
        {

            //Arrange
            Assembly? assembly = Assembly.GetAssembly(typeof(Hyme.Application.AssemblyReference));

            string[] otherProjects = new[]
            {
                APINamespace,
                InfrastructureNamespace,
                DomainNamespace,
                SharedNamesppace
            };

            //Act
            TestResult result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAll(otherProjects)
                .GetResult();

            //Assert
            result.IsSuccessful.Should().BeTrue();
        }

    }
}
