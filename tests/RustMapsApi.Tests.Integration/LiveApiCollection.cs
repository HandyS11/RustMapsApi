namespace RustMapsApi.Tests.Integration;

/// <summary>
/// Binds <see cref="LiveApiFixture"/> to a single non-parallel collection so all live
/// tests share one throttled client and never run concurrently against the API.
/// </summary>
[CollectionDefinition("LiveApi", DisableParallelization = true)]
public sealed class LiveApiCollection : ICollectionFixture<LiveApiFixture>;
