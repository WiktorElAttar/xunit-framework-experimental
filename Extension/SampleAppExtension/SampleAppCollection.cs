using Xunit;

namespace SampleAppExtension;

[CollectionDefinition("SampleApp Collection")]
public sealed class SampleAppCollectionFixture : ICollectionFixture<SampleAppFixture>;
