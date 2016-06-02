﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using NSubstitute;
using Octokit.Internal;
using Octokit.Reactive;
using Xunit;

namespace Octokit.Tests.Reactive
{
    public class ObservableMilestonesClientTests
    {
        public class TheGetMethod
        {
            [Fact]
            public void GetsFromClientIssueMilestone()
            {
                var gitHubClient = Substitute.For<IGitHubClient>();
                var client = new ObservableMilestonesClient(gitHubClient);

                client.Get("fake", "repo", 42);

                gitHubClient.Issue.Milestone.Received().Get("fake", "repo", 42);
            }

            [Fact]
            public async Task EnsuresNonNullArguments()
            {
                var client = new ObservableMilestonesClient(Substitute.For<IGitHubClient>());

                await Assert.ThrowsAsync<ArgumentNullException>(() => client.Get(null, "name", 1).ToTask());
                await Assert.ThrowsAsync<ArgumentNullException>(() => client.Get("owner", null, 1).ToTask());
                await Assert.ThrowsAsync<ArgumentNullException>(() => client.Get(null, "", 1).ToTask());
                await Assert.ThrowsAsync<ArgumentException>(() => client.Get("", null, 1).ToTask());
            }
        }

        public class TheGetForRepositoryMethod
        {
            [Fact]
            public void RequestsCorrectUrl()
            {
                var gitHubClient = Substitute.For<IGitHubClient>();
                var client = new ObservableMilestonesClient(gitHubClient);

                client.GetAllForRepository("fake", "repo");

                gitHubClient.Received().Issue.Milestone.GetAllForRepository("fake", "repo");
            }

            [Fact]
            public void RequestsCorrectUrlWithApiOptions()
            {
                var gitHubClient = Substitute.For<IGitHubClient>();
                var client = new ObservableMilestonesClient(gitHubClient);

                var options = new ApiOptions
                {
                    PageCount = 1,
                    PageSize = 1,
                    StartPage = 1
                };

                client.GetAllForRepository("fake", "repo", options);

                gitHubClient.Received().Issue.Milestone.GetAllForRepository("fake", "repo", options);
            }

            [Fact]
            public void SendsAppropriateParameters()
            {
                var gitHubClient = Substitute.For<IGitHubClient>();
                var client = new ObservableMilestonesClient(gitHubClient);

                var milestoneRequest = new MilestoneRequest { SortDirection = SortDirection.Descending };
                client.GetAllForRepository("fake", "repo", milestoneRequest);

                gitHubClient.Received().Issue.Milestone.GetAllForRepository("fake", "repo", milestoneRequest, Args.ApiOptions);
            }

            [Fact]
            public void SendsAppropriateParametersWithApiOptions()
            {
                var gitHubClient = Substitute.For<IGitHubClient>();
                var client = new ObservableMilestonesClient(gitHubClient);

                var options = new ApiOptions
                {
                    PageCount = 1,
                    PageSize = 1,
                    StartPage = 1
                };

                var milestoneRequest = new MilestoneRequest { SortDirection = SortDirection.Descending };
                client.GetAllForRepository("fake", "repo", milestoneRequest, options);

                gitHubClient.Received().Issue.Milestone.GetAllForRepository("fake", "repo", milestoneRequest, options);
            }

            [Fact]
            public void EnsuresNonNullArguments()
            {
                var client = new ObservableMilestonesClient(Substitute.For<IGitHubClient>());

                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository(null, null));
                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository(null, "name"));
                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository("owner", null));

                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository("owner", "name", (ApiOptions)null));
                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository("owner", "name", (MilestoneRequest)null));
                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository("owner", "name", new MilestoneRequest(), null));

                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository(null, "name", new ApiOptions()));
                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository("owner", null, new ApiOptions()));

                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository(null, "name", new MilestoneRequest()));
                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository("owner", null, new MilestoneRequest()));
                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository("owner", "name", (MilestoneRequest)null));

                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository("owner", "name", new MilestoneRequest(), null));
                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository("owner", "name", null, new ApiOptions()));
                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository("owner", null, new MilestoneRequest(), new ApiOptions()));
                Assert.Throws<ArgumentNullException>(() => client.GetAllForRepository(null, "name", new MilestoneRequest(), new ApiOptions()));

                Assert.Throws<ArgumentException>(() => client.GetAllForRepository("", ""));
                Assert.Throws<ArgumentException>(() => client.GetAllForRepository("", "name"));
                Assert.Throws<ArgumentException>(() => client.GetAllForRepository("owner", ""));

                Assert.Throws<ArgumentException>(() => client.GetAllForRepository("", "name", new ApiOptions()));
                Assert.Throws<ArgumentException>(() => client.GetAllForRepository("owner", "", new ApiOptions()));

                Assert.Throws<ArgumentException>(() => client.GetAllForRepository("", "name", new MilestoneRequest()));
                Assert.Throws<ArgumentException>(() => client.GetAllForRepository("owner", "", new MilestoneRequest()));

                Assert.Throws<ArgumentException>(() => client.GetAllForRepository("owner", "", new MilestoneRequest(), new ApiOptions()));
                Assert.Throws<ArgumentException>(() => client.GetAllForRepository("", "name", new MilestoneRequest(), new ApiOptions()));
            }

            [Fact]
            public void ReturnsEveryPageOfMilestones()
            {
                var firstPageUrl = new Uri("repos/fake/repo/milestones", UriKind.Relative);
                var secondPageUrl = new Uri("https://example.com/page/2");
                var firstPageLinks = new Dictionary<string, Uri> { { "next", secondPageUrl } };
                var firstPageResponse = new ApiResponse<List<Milestone>>
                (
                    CreateResponseWithApiInfo(firstPageLinks),
                    new List<Milestone>
                    {
                        new Milestone(1),
                        new Milestone(2),
                        new Milestone(3)
                    }
                );
                var thirdPageUrl = new Uri("https://example.com/page/3");
                var secondPageLinks = new Dictionary<string, Uri> { { "next", thirdPageUrl } };
                var secondPageResponse = new ApiResponse<List<Milestone>>
                (
                    CreateResponseWithApiInfo(secondPageLinks),
                    new List<Milestone>
                    {
                        new Milestone(4),
                        new Milestone(5),
                        new Milestone(6)
                    }
                );
                var lastPageResponse = new ApiResponse<List<Milestone>>
                (
                    new Response(),
                    new List<Milestone>
                    {
                        new Milestone(7)
                    }
               );
                var gitHubClient = Substitute.For<IGitHubClient>();
                gitHubClient.Connection.Get<List<Milestone>>(firstPageUrl, null, null)
                    .Returns(Task.Factory.StartNew<IApiResponse<List<Milestone>>>(() => firstPageResponse));
                gitHubClient.Connection.Get<List<Milestone>>(secondPageUrl, null, null)
                    .Returns(Task.Factory.StartNew<IApiResponse<List<Milestone>>>(() => secondPageResponse));
                gitHubClient.Connection.Get<List<Milestone>>(thirdPageUrl, null, null)
                    .Returns(Task.Factory.StartNew<IApiResponse<List<Milestone>>>(() => lastPageResponse));
                var client = new ObservableMilestonesClient(gitHubClient);

                var results = client.GetAllForRepository("fake", "repo").ToArray().Wait();

                Assert.Equal(7, results.Length);
                Assert.Equal(firstPageResponse.Body[0].Number, results[0].Number);
                Assert.Equal(secondPageResponse.Body[1].Number, results[4].Number);
                Assert.Equal(lastPageResponse.Body[0].Number, results[6].Number);
            }

            [Fact]
            public async Task SendsAppropriateParametersMulti()
            {
                var firstPageUrl = new Uri("repos/fake/repo/milestones", UriKind.Relative);
                var secondPageUrl = new Uri("https://example.com/page/2");
                var firstPageLinks = new Dictionary<string, Uri> { { "next", secondPageUrl } };
                var firstPageResponse = new ApiResponse<List<Milestone>>
                (
                    CreateResponseWithApiInfo(firstPageLinks),
                    new List<Milestone>
                    {
                        new Milestone(1),
                        new Milestone(2),
                        new Milestone(3)
                    }
                );
                var thirdPageUrl = new Uri("https://example.com/page/3");
                var secondPageLinks = new Dictionary<string, Uri> { { "next", thirdPageUrl } };
                var secondPageResponse = new ApiResponse<List<Milestone>>
                (
                    CreateResponseWithApiInfo(secondPageLinks),
                    new List<Milestone>
                    {
                        new Milestone(4),
                        new Milestone(5),
                        new Milestone(6)
                    }
                );
                var lastPageResponse = new ApiResponse<List<Milestone>>
                (
                    new Response(),
                    new List<Milestone>
                    {
                        new Milestone(7)
                    }
                );
                var gitHubClient = Substitute.For<IGitHubClient>();
                gitHubClient.Connection.Get<List<Milestone>>(Arg.Is(firstPageUrl),
                    Arg.Is<Dictionary<string, string>>(d => d.Count == 3
                        && d["direction"] == "desc"
                        && d["state"] == "open"
                        && d["sort"] == "due_date"), Arg.Any<string>())
                    .Returns(Task.Factory.StartNew<IApiResponse<List<Milestone>>>(() => firstPageResponse));
                gitHubClient.Connection.Get<List<Milestone>>(secondPageUrl, null, null)
                    .Returns(Task.Factory.StartNew<IApiResponse<List<Milestone>>>(() => secondPageResponse));
                gitHubClient.Connection.Get<List<Milestone>>(thirdPageUrl, null, null)
                    .Returns(Task.Factory.StartNew<IApiResponse<List<Milestone>>>(() => lastPageResponse));
                var client = new ObservableMilestonesClient(gitHubClient);

                var results = await client.GetAllForRepository("fake", "repo", new MilestoneRequest { SortDirection = SortDirection.Descending }).ToArray();

                Assert.Equal(7, results.Length);
                Assert.Equal(firstPageResponse.Body[0].Number, results[0].Number);
                Assert.Equal(secondPageResponse.Body[1].Number, results[4].Number);
                Assert.Equal(lastPageResponse.Body[0].Number, results[6].Number);
            }
        }

        public class TheCreateMethod
        {
            [Fact]
            public void CreatesFromClientIssueMilestone()
            {
                var newMilestone = new NewMilestone("some title");
                var gitHubClient = Substitute.For<IGitHubClient>();
                var client = new ObservableMilestonesClient(gitHubClient);

                client.Create("fake", "repo", newMilestone);

                gitHubClient.Issue.Milestone.Received().Create("fake", "repo", newMilestone);
            }

            [Fact]
            public void EnsuresArgumentsNotNull()
            {
                var gitHubClient = Substitute.For<IGitHubClient>();
                var client = new ObservableMilestonesClient(gitHubClient);

                Assert.Throws<ArgumentNullException>(() => client.Create(null, "name", new NewMilestone("title")));
                Assert.Throws<ArgumentException>(() => client.Create("", "name", new NewMilestone("x")));
                Assert.Throws<ArgumentNullException>(() => client.Create("owner", null, new NewMilestone("x")));
                Assert.Throws<ArgumentException>(() => client.Create("owner", "", new NewMilestone("x")));
                Assert.Throws<ArgumentNullException>(() => client.Create("owner", "name", null));
            }
        }

        public class TheUpdateMethod
        {
            [Fact]
            public void UpdatesClientIssueMilestone()
            {
                var milestoneUpdate = new MilestoneUpdate();
                var gitHubClient = Substitute.For<IGitHubClient>();
                var client = new ObservableMilestonesClient(gitHubClient);

                client.Update("fake", "repo", 42, milestoneUpdate);

                gitHubClient.Issue.Milestone.Received().Update("fake", "repo", 42, milestoneUpdate);
            }

            [Fact]
            public void EnsuresArgumentsNotNull()
            {
                var gitHubClient = Substitute.For<IGitHubClient>();
                var client = new ObservableMilestonesClient(gitHubClient);

                Assert.Throws<ArgumentNullException>(() => client.Update(null, "name", 42, new MilestoneUpdate()));
                Assert.Throws<ArgumentException>(() => client.Update("", "name", 42, new MilestoneUpdate()));
                Assert.Throws<ArgumentNullException>(() => client.Update("owner", null, 42, new MilestoneUpdate()));
                Assert.Throws<ArgumentException>(() => client.Update("owner", "", 42, new MilestoneUpdate()));
                Assert.Throws<ArgumentNullException>(() => client.Update("owner", "name", 42, null));
            }
        }

        public class TheDeleteMethod
        {
            [Fact]
            public void DeletesFromClientIssueMilestone()
            {
                var gitHubClient = Substitute.For<IGitHubClient>();
                var client = new ObservableMilestonesClient(gitHubClient);

                client.Delete("fake", "repo", 42);

                gitHubClient.Issue.Milestone.Received().Delete("fake", "repo", 42);
            }

            [Fact]
            public void EnsuresArgumentsNotNull()
            {
                var gitHubClient = Substitute.For<IGitHubClient>();
                var client = new ObservableMilestonesClient(gitHubClient);

                Assert.Throws<ArgumentNullException>(() => client.Delete(null, "name", 42));
                Assert.Throws<ArgumentException>(() => client.Delete("", "name", 42));
                Assert.Throws<ArgumentNullException>(() => client.Delete("owner", null, 42));
                Assert.Throws<ArgumentException>(() => client.Delete("owner", "", 42));
            }
        }

        public class TheCtor
        {
            [Fact]
            public void EnsuresNonNullArguments()
            {
                Assert.Throws<ArgumentNullException>(() => new ObservableMilestonesClient(null));
            }
        }

        static IResponse CreateResponseWithApiInfo(IDictionary<string, Uri> links)
        {
            var response = Substitute.For<IResponse>();
            response.ApiInfo.Returns(new ApiInfo(links, new List<string>(), new List<string>(), "etag", new RateLimit(new Dictionary<string, string>())));
            return response;
        }
    }
}
