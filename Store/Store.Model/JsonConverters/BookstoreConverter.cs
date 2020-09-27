using System;
using Newtonsoft.Json.Converters;

using Store.Models;
using Store.Model.Common.Models;

namespace Store.Model.JsonConverters
{
    public class BookstoreConverter : CustomCreationConverter<IBookstore>
    {
        public override IBookstore Create(Type objectType)
        {
            return new Bookstore();
        }
    }
}