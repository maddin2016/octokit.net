﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Octokit
{
    /// <summary>
    /// A client for GitHub's Invitations on a Repository.
    /// </summary>
    /// <remarks>
    /// See the <a href="https://developer.github.com/v3/repos/invitations/">Invitations API documentation</a> for more details.
    /// </remarks>
    public interface IRepositoryInvitationsClient
    {
        /// <summary>
        /// Accept a repository invitation.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://developer.github.com/v3/repos/invitations/#accept-a-repository-invitation">API documentation</a> for more information.
        /// </remarks>        
        /// <param name="id">The id of the invitation</param>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>        
        /// <returns><see cref="Task"/></returns>
        Task Accept(int id);

        /// <summary>
        /// Decline a repository invitation.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://developer.github.com/v3/repos/invitations/#decline-a-repository-invitation">API documentation</a> for more information.
        /// </remarks>        
        /// <param name="id">The id of the invitation</param>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>        
        /// <returns><see cref="Task"/></returns>
        Task Decline(int id);

        /// <summary>
        /// Deletes a repository invitation.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://developer.github.com/v3/repos/invitations/#delete-a-repository-invitation">API documentation</a> for more information.
        /// </remarks>
        /// <param name="repositoryId">The id ot the repository</param>
        /// <param name="invitationId">The id of the invitation</param>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns><see cref="Task"/></returns>
        Task Delete(int repositoryId, int invitationId);

        /// <summary>
        /// Gets all invitations for the current user.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://developer.github.com/v3/repos/invitations/#list-a-users-repository-invitations">API documentation</a> for more information.
        /// </remarks>        
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A <see cref="IReadOnlyList{RepositoryInvitation}"/> of <see cref="RepositoryInvitation"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Task<IReadOnlyList<RepositoryInvitation>> GetAllForCurrent();

        /// <summary>
        /// Gets all the invitations on a repository.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://developer.github.com/v3/repos/invitations/#list-invitations-for-a-repository">API documentation</a> for more information.
        /// </remarks>        
        /// <param name="repositoryId">The id of the repository</param>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A <see cref="IReadOnlyList{RepositoryInvitation}"/> of <see cref="RepositoryInvitation"/>.</returns>
        Task<IReadOnlyList<RepositoryInvitation>> GetAllForRepository(int repositoryId);

        /// <summary>
        /// Updates a repository invitation.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://developer.github.com/v3/repos/invitations/#update-a-repository-invitation">API documentation</a> for more information.
        /// </remarks>
        /// <param name="repositoryId">The id ot the repository</param>
        /// <param name="invitationId">The id of the invitation</param>
        /// <param name="permissions">The permission for the collsborator</param>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns><see cref="RepositoryInvitation"/></returns>
        Task<RepositoryInvitation> Edit(int repositoryId, int invitationId, InvitationUpdate permissions);
    }
}
