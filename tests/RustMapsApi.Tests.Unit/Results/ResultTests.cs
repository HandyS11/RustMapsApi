using RustMapsApi.Results;

namespace RustMapsApi.Tests.Unit.Results;

public class ResultTests
{
    [Fact]
    public void Success_PopulatesDataAndStatus_LeavesErrorNull()
    {
        var result = Result<int>.Success(42, 200);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Data);
        Assert.Null(result.Error);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void Failure_PopulatesErrorAndStatus_LeavesDataDefault()
    {
        var error = new RustMapsError(RustMapsErrorKind.NotFound, "missing", null, null);

        var result = Result<string>.Failure(error, 404);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Same(error, result.Error);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public void IsSuccess_NarrowsDataAndErrorNullability()
    {
        // Compile-time contract: checking IsSuccess must narrow Data/Error to non-null for callers.
        // Without [MemberNotNullWhen] on IsSuccess, the dereferences below are CS8602 and fail the build.
        var success = Result<string>.Success("payload", 200);
        var failure = Result<string>.Failure(new RustMapsError(RustMapsErrorKind.NotFound, "missing", null, null), 404);

        Assert.True(success.IsSuccess);
        Assert.False(failure.IsSuccess);
        if (success.IsSuccess)
        {
            Assert.Equal(7, success.Data.Length);
        }

        if (!failure.IsSuccess)
        {
            Assert.Equal(RustMapsErrorKind.NotFound, failure.Error.Kind);
        }
    }
}
