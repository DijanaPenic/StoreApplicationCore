using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models;

namespace Store.Repository.Common.Repositories
{
    public interface IEmailTemplateRepository : IRepository<IEmailTemplate, Guid>
    {
        Task<IEmailTemplate> FindByClientIdAsync(Guid clientId, EmailTemplateType emailTemplateType);

        Task<IEnumerable<IEmailTemplate>> FindByClientIdAsync(Guid clientId);

        Task<bool> ExistsAsync(Guid emailTemplateId);

        Task<bool> ExistsAsync(Guid clientId, EmailTemplateType emailTemplateType);

        Task<IPagedList<IEmailTemplate>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null);
    }
}