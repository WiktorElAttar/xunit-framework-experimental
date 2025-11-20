using Xunit;

namespace SampleApp.Tests;

[CollectionDefinition("SampleApp Collection")]
public sealed class SampleAppCollectionFixture : ICollectionFixture<SampleAppFixture>;
