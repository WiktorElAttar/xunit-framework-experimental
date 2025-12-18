using Xunit;
using XUnitFramework.Extension;

namespace SampleAppExtension;

[Collection("SampleApp Collection")]
public abstract class SampleAppTestBase : BaseIntegrationTest<Program, SampleAppFixture>;
