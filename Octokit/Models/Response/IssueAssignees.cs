﻿using System;
using System.Collections.Generic;

namespace Octokit
{
    public class IssueAssignees : Issue
    {
        public IssueAssignees(Uri url, Uri htmlUrl, Uri commentsUrl, Uri eventsUrl, int number, ItemState state, string title, string body, User user, IReadOnlyList<Label> labels, User assignee, Milestone milestone, int comments, PullRequest pullRequest, DateTimeOffset? closedAt, DateTimeOffset createdAt, DateTimeOffset? updatedAt, int id, bool locked, Repository repository, IReadOnlyList<User> assignees)
            : base(url, htmlUrl, commentsUrl, eventsUrl, number, state, title, body, user, labels, assignee, milestone, comments, pullRequest, closedAt, createdAt, updatedAt, id, locked, repository)
        {
            Assignees = assignees;
        }

        /// <summary>
        /// List of of assigned users for this issue.
        /// </summary>
        public IReadOnlyList<User> Assignees { get; protected set; }
    }
}
