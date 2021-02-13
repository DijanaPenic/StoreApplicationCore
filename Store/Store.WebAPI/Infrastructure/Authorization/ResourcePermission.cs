using System.ComponentModel;

namespace Store.WebAPI.Infrastructure.Authorization
{
    public enum ResourcePermission
    {
		[Description("Books.Read")]
		BooksRead,

		[Description("Books.Create")]
		BooksCreate,

		[Description("Books.Update")]
		BooksUpdate,

		[Description("Books.Delete")]
		BooksDelete
	}
}