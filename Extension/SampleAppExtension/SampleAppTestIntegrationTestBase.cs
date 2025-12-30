using Xunit;
using XUnitFramework.Extension;

namespace SampleAppExtension;

[Collection("SampleApp Collection")]
public abstract class SampleAppTestIntegrationTestBase : IntegrationTestBase<Program, SampleAppFixtureBase>;
