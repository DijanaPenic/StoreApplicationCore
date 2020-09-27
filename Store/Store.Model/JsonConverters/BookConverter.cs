using System;
using Newtonsoft.Json.Converters;

using Store.Models;
using Store.Model.Common.Models;

namespace Store.Model.JsonConverters
{
    public class BookConverter : CustomCreationConverter<IBook>
    {
        public override IBook Create(Type objectType)
        {
            return new Book();
        }
    }
}