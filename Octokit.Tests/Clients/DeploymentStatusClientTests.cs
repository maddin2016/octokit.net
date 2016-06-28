﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using Octokit;
using Octokit.Tests;
using Xunit;

public class DeploymentStatusClientTests
{
    public class TheGetAllMethod
    {
        [Fact]
        public async Task EnsuresNonNullArguments()
        {
            var client = new DeploymentStatusClient(Substitute.For<IApiConnection>());

            await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetAll(null, "name", 1));
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetAll("owner", null, 1));

            await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetAll(null, "name", 1, ApiOptions.None));
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetAll("owner", null, 1, ApiOptions.None));
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetAll("owner", "name", 1, null));
        }

        [Fact]
        public async Task EnsuresNonEmptyArguments()
        {
            var client = new DeploymentStatusClient(Substitute.For<IApiConnection>());

            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll("", "name", 1));
            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll("owner", "", 1));

            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll("", "name", 1, ApiOptions.None));
            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll("owner", "", 1, ApiOptions.None));
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("  ")]
        [InlineData("\n\r")]
        public async Task EnsureNonWhitespaceArguments(string whitespace)
        {
            var client = new DeploymentStatusClient(Substitute.For<IApiConnection>());

            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll(whitespace, "name", 1));
            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll("owner", whitespace, 1));

            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll(whitespace, "name", 1, ApiOptions.None));
            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll("owner", whitespace, 1, ApiOptions.None));
        }

        [Fact]
        public void RequestsCorrectUrl()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new DeploymentStatusClient(connection);
            var expectedUrl = "repos/owner/name/deployments/1/statuses";

            client.GetAll("owner", "name", 1);
            connection.Received().GetAll<DeploymentStatus>(Arg.Is<Uri>(u => u.ToString() == expectedUrl), 
                                                           Arg.Any<IDictionary<string, string>>(),
                                                           Arg.Any<string>(),
                                                           Args.ApiOptions);
        }

        [Fact]
        public void RequestsCorrectUrlWithApiOptions()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new DeploymentStatusClient(connection);
            var expectedUrl = "repos/owner/name/deployments/1/statuses";

            var options = new ApiOptions
            {
                StartPage = 1,
                PageCount = 1,
                PageSize = 1
            };

            client.GetAll("owner", "name", 1, options);
            connection.Received().GetAll<DeploymentStatus>(Arg.Is<Uri>(u => u.ToString() == expectedUrl),
                                                           Arg.Any<IDictionary<string, string>>(),
                                                           Arg.Any<string>(), 
                                                           options);
        }

        [Fact]
        public void RequestsCorrectUrlWithPreviewAcceptHeaders()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new DeploymentStatusClient(connection);
            var expectedUrl = "repos/owner/name/deployments/1/statuses";

            client.GetAll("owner", "name", 1);
            connection.Received().GetAll<DeploymentStatus>(Arg.Is<Uri>(u => u.ToString() == expectedUrl),
                                                           Arg.Any<IDictionary<string, string>>(),
                                                           Arg.Is<string>(a => a == AcceptHeaders.DeploymentApiPreview),
                                                           Args.ApiOptions);
        }
    }

    public class TheCreateMethod
    {
        readonly NewDeploymentStatus newDeploymentStatus = new NewDeploymentStatus(DeploymentState.Success);

        [Fact]
        public async Task EnsuresNonNullArguments()
        {
            var client = new DeploymentStatusClient(Substitute.For<IApiConnection>());

            await Assert.ThrowsAsync<ArgumentNullException>(() => client.Create(null, "name", 1, newDeploymentStatus));
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.Create("owner", null, 1, newDeploymentStatus));
        }

        [Fact]
        public async Task EnsuresNonEmptyArguments()
        {
            var client = new DeploymentStatusClient(Substitute.For<IApiConnection>());

            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll("", "name", 1));
            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll("owner", "", 1));
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("  ")]
        [InlineData("\n\r")]
        public async Task EnsureNonWhitespaceArguments(string whitespace)
        {
            var client = new DeploymentStatusClient(Substitute.For<IApiConnection>());

            await Assert.ThrowsAsync<ArgumentException>(() => client.Create(whitespace, "repo", 1, newDeploymentStatus));
            await Assert.ThrowsAsync<ArgumentException>(() => client.Create("owner", whitespace, 1, newDeploymentStatus));
        }

        [Fact]
        public void PostsToCorrectUrl()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new DeploymentStatusClient(connection);
            var expectedUrl = "repos/owner/repo/deployments/1/statuses";

            client.Create("owner", "repo", 1, newDeploymentStatus);

            connection.Received().Post<DeploymentStatus>(Arg.Is<Uri>(u => u.ToString() == expectedUrl),
                                                         Arg.Any<NewDeploymentStatus>(),
                                                         Arg.Any<string>());
        }

        [Fact]
        public void PassesNewDeploymentRequest()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new DeploymentStatusClient(connection);
            var expectedUrl = "repos/owner/repo/deployments/1/statuses";

            client.Create("owner", "repo", 1, newDeploymentStatus);

            connection.Received().Post<DeploymentStatus>(Arg.Is<Uri>(u => u.ToString() == expectedUrl),
                                                         newDeploymentStatus,
                                                         Arg.Any<string>());
        }

        [Fact]
        public void SendsPreviewAcceptHeaders()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new DeploymentStatusClient(connection);
            var expectedUrl = "repos/owner/repo/deployments/1/statuses";

            client.Create("owner", "repo", 1, newDeploymentStatus);

            connection.Received(1).Post<DeploymentStatus>(Arg.Is<Uri>(u => u.ToString() == expectedUrl),
                                                          Arg.Any<NewDeploymentStatus>(),
                                                          Arg.Is<string>(s => s == AcceptHeaders.DeploymentApiPreview));
        }
    }

    public class TheCtor
    {
        [Fact]
        public void EnsuresNonNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new DeploymentStatusClient(null));
        }
    }
}