using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Models;
using Store.Model.Common.Models;
using Store.Common.Enums;
using Store.Repository.Common.Core;
using Store.Service.Common.Services;

namespace Store.Services
{
    public class GlobalSearchService : IGlobalSearchService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GlobalSearchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ISearchItem>> FindAsync(string searchString, IList<ModuleType> searchTypes)
        {
            IList<ISearchItem> searchItems = new List<ISearchItem>();

            if (searchTypes.Contains(ModuleType.Book))
            {
                IEnumerable<IBook> books = await _unitOfWork.BookRepository.FindAsync
                (
                    b => b.Name.Contains(searchString) || b.Author.Contains(searchString) || b.Bookstore.Name.Contains(searchString),
                    true,
                    nameof(IBook.Name),
                    nameof(IBook.Bookstore)
                );

                foreach (IBook book in books)
                {
                    ISearchItem searchItem = new SearchItem
                    {
                        Id = book.Id,
                        Name = $"{book.Name} ({book.Bookstore.Name})",
                        Type = ModuleType.Book
                    };

                    searchItems.Add(searchItem);
                }
            }

            if (searchTypes.Contains(ModuleType.Bookstore))
            {
                IEnumerable<IBookstore> bookstores = await _unitOfWork.BookstoreRepository.FindAsync
                (
                    bs => bs.Name.Contains(searchString) || bs.Location.Contains(searchString),
                    true,
                    nameof(IBookstore.Name)
                );

                foreach (IBookstore bookstore in bookstores)
                {
                    ISearchItem searchItem = new SearchItem
                    {
                        Id = bookstore.Id,
                        Name = bookstore.Name,
                        Type = ModuleType.Bookstore
                    };

                    searchItems.Add(searchItem);
                }
            }

            return searchItems;
        }
    }
}