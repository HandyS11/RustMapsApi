namespace RustMapsApi.Tests.Results;

using RustMapsApi.Results;

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
}
