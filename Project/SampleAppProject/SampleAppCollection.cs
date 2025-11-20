using Xunit;

namespace SampleAppProject;

[CollectionDefinition("SampleApp Collection")]
public sealed class SampleAppCollectionFixture : ICollectionFixture<SampleAppFixture>;
