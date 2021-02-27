using AutoMapper;
using System.Linq;

using Store.WebAPI.Models.Identity;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Mapper.Converters
{
    public class PolicyConverter<TSource, TDestination> : ITypeConverter<IRole, RoleGetApiModel>
    {
        public RoleGetApiModel Convert(IRole source, RoleGetApiModel destination, ResolutionContext context)
        {
            PolicyGetApiModel[] dstPolicies = source.Policies.GroupBy(rc => rc.ClaimValue.Split('.')[0])
                                                             .Select(rcg => new PolicyGetApiModel 
                                                             { 
                                                                 Section = rcg.Key, 
                                                                 AccessActions = rcg.Select(a => new AccessActionGetApiModel 
                                                                                    { 
                                                                                        Id = a.Id, 
                                                                                        DateCreatedUtc = a.DateCreatedUtc, 
                                                                                        Name = a.ClaimValue.Split('.')[1] 
                                                                                    }).ToArray() 
                                                             }).ToArray();           
            
            return new RoleGetApiModel
            {
                Id = source.Id,
                Name = source.Name,
                Stackable = source.Stackable,
                DateCreatedUtc = source.DateCreatedUtc,
                DateUpdatedUtc = source.DateUpdatedUtc,
                Policies = dstPolicies
            };
        }
    }
}