using System;

using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models.Identity
{
    public interface IUserToken : IUserTokenKey, IBaseEntity, IChangable
    {
        string Value { get; set; }
    }

    public interface IUserTokenKey
    {
        Guid UserId { get; set; }

        string LoginProvider { get; set; }

        string Name { get; set; }

        public object[] ToArray();
    }
}