using System.Net;
using OpsFlow.IntegrationTests.Infrastructure;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace OpsFlow.IntegrationTests.Api;

public sealed class FilesTests(OpsFlowWebApplicationFactory factory)
    : IClassFixture<OpsFlowWebApplicationFactory>
{
    [Fact]
    public async Task OwnerCanUploadListDownloadAndDeleteFile()
    {
        var auth = await RegisterAndLoginAsync(
            $"file-{Guid.NewGuid():N}@example.com");

        var organizationId = await CreateOrganizationAsync(auth.AccessToken);

        const string fileName = "hello.txt";
        const string contentType = "text/plain";
        const string content = "OpsFlow file integration test.";

        using var uploadContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(
            System.Text.Encoding.UTF8.GetBytes(content));

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(contentType);

        uploadContent.Add(fileContent, "file", fileName);

        var upload = await SendMultipartAsync(
            HttpMethod.Post,
            "/api/v1/files",
            auth.AccessToken,
            organizationId,
            uploadContent);

        upload.StatusCode.Should().Be(HttpStatusCode.Created);

        var uploaded =
            await upload.Content.ReadFromJsonAsync<UploadFileResponse>();

        uploaded.Should().NotBeNull();
        uploaded!.OriginalFileName.Should().Be(fileName);
        uploaded.ContentType.Should().Be(contentType);
        uploaded.SizeBytes.Should().Be(
            System.Text.Encoding.UTF8.GetByteCount(content));
        uploaded.OrganizationId.Should().Be(organizationId);
        uploaded.UploadedByUserId.Should().Be(auth.UserId);

        var list = await SendAsync(
            HttpMethod.Get,
            "/api/v1/files",
            auth.AccessToken,
            organizationId);

        list.StatusCode.Should().Be(HttpStatusCode.OK);

        var listed =
            await list.Content.ReadFromJsonAsync<ListFilesResponse>();

        listed.Should().NotBeNull();
        listed!.Items.Should().ContainSingle();
        listed.Items[0].Id.Should().Be(uploaded.Id);
        listed.Items[0].OriginalFileName.Should().Be(fileName);

        var download = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/files/{uploaded.Id}",
            auth.AccessToken,
            organizationId);

        download.StatusCode.Should().Be(HttpStatusCode.OK);
        download.Content.Headers.ContentType?.MediaType
            .Should().Be(contentType);

        var downloadedBytes = await download.Content.ReadAsByteArrayAsync();

        System.Text.Encoding.UTF8.GetString(downloadedBytes)
            .Should().Be(content);

        var delete = await SendAsync(
            HttpMethod.Delete,
            $"/api/v1/files/{uploaded.Id}",
            auth.AccessToken,
            organizationId);

        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var listAfterDelete = await SendAsync(
            HttpMethod.Get,
            "/api/v1/files",
            auth.AccessToken,
            organizationId);

        listAfterDelete.StatusCode.Should().Be(HttpStatusCode.OK);

        var afterDelete =
            await listAfterDelete.Content.ReadFromJsonAsync<ListFilesResponse>();

        afterDelete.Should().NotBeNull();
        afterDelete!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task FilesAreIsolatedBetweenOrganizations()
    {
        var auth = await RegisterAndLoginAsync(
            $"file-isolation-{Guid.NewGuid():N}@example.com");

        var organizationA = await CreateOrganizationAsync(auth.AccessToken);
        var organizationB = await CreateOrganizationAsync(auth.AccessToken);

        using var uploadContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(
            System.Text.Encoding.UTF8.GetBytes("Organization A file"));

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue("text/plain");

        uploadContent.Add(fileContent, "file", "org-a.txt");

        var upload = await SendMultipartAsync(
            HttpMethod.Post,
            "/api/v1/files",
            auth.AccessToken,
            organizationA,
            uploadContent);

        upload.StatusCode.Should().Be(HttpStatusCode.Created);

        var uploaded =
            await upload.Content.ReadFromJsonAsync<UploadFileResponse>();

        uploaded.Should().NotBeNull();

        var organizationBList = await SendAsync(
            HttpMethod.Get,
            "/api/v1/files",
            auth.AccessToken,
            organizationB);

        organizationBList.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await organizationBList.Content.ReadFromJsonAsync<ListFilesResponse>();

        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();

        var crossTenantDownload = await SendAsync(
            HttpMethod.Get,
            $"/api/v1/files/{uploaded!.Id}",
            auth.AccessToken,
            organizationB);

        crossTenantDownload.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<AuthResult> RegisterAndLoginAsync(string email)
    {
        const string password = "Password123!";

        var client = factory.CreateClient();

        var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password,
                firstName = "Test",
                lastName = "User"
            });

        register.StatusCode.Should().Be(HttpStatusCode.Created);

        var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email,
                password
            });

        login.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await login.Content.ReadFromJsonAsync<LoginResponse>();

        result.Should().NotBeNull();

        return new AuthResult(
            result!.AccessToken,
            result.UserId);
    }

    private async Task<Guid> CreateOrganizationAsync(string accessToken)
    {
        var response = await SendAsync(
            HttpMethod.Post,
            "/api/v1/organizations",
            accessToken,
            null,
            new
            {
                name = $"Files Organization {Guid.NewGuid():N}",
                slug = $"files-{Guid.NewGuid():N}"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result =
            await response.Content.ReadFromJsonAsync<OrganizationResponse>();

        result.Should().NotBeNull();

        return result!.OrganizationId;
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string url,
        string accessToken,
        Guid? organizationId,
        object? body = null)
    {
        var client = factory.CreateClient();

        using var request = new HttpRequestMessage(method, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        if (organizationId.HasValue)
        {
            request.Headers.Add(
                "X-Organization-Id",
                organizationId.Value.ToString());
        }

        if (body is not null)
            request.Content = JsonContent.Create(body);

        return await client.SendAsync(request);
    }

    private async Task<HttpResponseMessage> SendMultipartAsync(
        HttpMethod method,
        string url,
        string accessToken,
        Guid organizationId,
        HttpContent content)
    {
        var client = factory.CreateClient();

        using var request = new HttpRequestMessage(method, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        request.Headers.Add(
            "X-Organization-Id",
            organizationId.ToString());

        request.Content = content;

        return await client.SendAsync(request);
    }

    private sealed record AuthResult(
        string AccessToken,
        Guid UserId);

    private sealed record LoginResponse(
        string AccessToken,
        Guid UserId);

    private sealed record OrganizationResponse(
        Guid OrganizationId);

    private sealed record UploadFileResponse(
        Guid Id,
        Guid OrganizationId,
        Guid UploadedByUserId,
        string OriginalFileName,
        string ContentType,
        long SizeBytes,
        DateTimeOffset CreatedAtUtc);

    private sealed record ListFilesResponse(
        IReadOnlyList<FileItemResponse> Items,
        int Page,
        int PageSize);

    private sealed record FileItemResponse(
        Guid Id,
        Guid OrganizationId,
        Guid UploadedByUserId,
        string OriginalFileName,
        string ContentType,
        long SizeBytes,
        DateTimeOffset CreatedAtUtc);
}
