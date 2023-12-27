using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using IPALogger = IPA.Logging.Logger;

namespace ConfigLimitFixer.Tests;

// Base class for all unit tests.
// This class is abstract.
// It internally initializes AutoFixture with NSubstitute, while the derived class can use the Fixture property to create objects.
// It is also possible to customize AutoFixture with NSubstitute (AutoNSubstituteCustomization) in the derived class.
public class UnitTestsBase
{
    protected IFixture Fixture { get; }

    protected UnitTestsBase()
    {
        this.Fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
    }

    protected UnitTestsBase(params ICustomization[] customizations)
    {
        var fixture = new Fixture();
        foreach (var customization in customizations)
        {
            fixture.Customize(customization);
        }

        this.Fixture = fixture.Customize(new AutoNSubstituteCustomization());
    }

    protected UnitTestsBase(Action<IFixture> autoNSubstituteCustomization)
    {
        var fixture = new Fixture();
        autoNSubstituteCustomization?.Invoke(fixture);
        this.Fixture = fixture;
    }

    protected UnitTestsBase(
        Action<IFixture> autoNSubstituteCustomization,
        ICustomization customization)
    {
        var fixture = new Fixture();
        autoNSubstituteCustomization?.Invoke(fixture);
        this.Fixture = fixture.Customize(customization);
    }

    protected UnitTestsBase(
        Action<IFixture> autoNSubstituteCustomization,
        AutoNSubstituteCustomization autoNSubstituteCustomization2)
    {
        var fixture = new Fixture();
        autoNSubstituteCustomization?.Invoke(fixture);
        this.Fixture = fixture.Customize(autoNSubstituteCustomization2);
    }

    protected IPALogger MockIpaLogger()
    {
        var ipaLogger = this.Fixture.Create<IPALogger>();
        return ipaLogger;
    }
}