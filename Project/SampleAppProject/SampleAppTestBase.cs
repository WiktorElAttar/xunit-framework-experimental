using Xunit;
using XUnitFramework.Project;

namespace SampleAppProject;

[Collection("SampleApp Collection")]
public abstract class SampleAppTestBase(SampleAppFixture fixture)
    : BaseIntegrationTest<Program>(fixture);
