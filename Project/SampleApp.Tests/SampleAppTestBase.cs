using Xunit;
using XUnitFramework.Project;

namespace SampleApp.Tests;

[Collection("SampleApp Collection")]
public abstract class SampleAppTestBase(SampleAppFixture fixture)
    : BaseIntegrationTest<Program>(fixture);
