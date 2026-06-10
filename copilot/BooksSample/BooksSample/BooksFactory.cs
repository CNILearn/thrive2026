using System;
using System.Collections.Generic;
using System.Text;

namespace BooksSample;

internal class BooksFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Book> GetSampleBooks()
    {
        return new List<Book>
        {
            new Book
            {
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                Description = "A novel about the American dream and the decadence of the Jazz Age."
            },
            new Book
            {
                Title = "To Kill a Mockingbird",
                Author = "Harper Lee",
                Description = "A novel about racial injustice in the Deep South."
            },
            new Book
            {
                Title = "1984",
                Author = "George Orwell",
                Description = "A dystopian novel about totalitarianism and  "
            }
        };
    }
}
