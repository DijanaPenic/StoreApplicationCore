﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IApplicationUserStore : IUserStore<IUser>, IDisposable
    {
        Task<IPagedEnumerable<IUser>> FindUsersAsync(IUserFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options, CancellationToken cancellationToken);

        Task<IUser> FindUserByIdAsync(Guid id, IOptionsParameters options, CancellationToken cancellationToken);

        Task ApproveUserAsync(IUser user, CancellationToken cancellationToken);

        Task DisapproveUserAsync(IUser user, CancellationToken cancellationToken);
    }
}