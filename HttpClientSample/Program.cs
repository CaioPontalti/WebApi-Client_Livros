using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


namespace HttpClientSample
{
    class Program
    {
        private class Book
        {
            public int BookId { get; set; }
            public string Title { get; set; }
            public decimal Price { get; set; }
            public string Genre { get; set; }
            public DateTime PublishDate { get; set; }
            public string Description { get; set; }
            public int AuthorId { get; set; }
            public Author Author { get; set; }

            public override string ToString()
            {
                return this.Title;
              
            }

            public Book()
            {
                Author Autor = new Author();
            }
        }
        public class Author
        {
            public int AuthorId { get; set; }         
            public string Name { get; set; }
        }
        
        private readonly Expression<Func<Book, BookDto>> AsBookDto = x => new BookDto
        {
            Title = x.Title,
            Author = x.Author.Name,
            Genre = x.Genre
        };

        private static HttpClient client = new HttpClient();

        static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            string operacao = "Get";

            client.BaseAddress = new Uri("http://localhost:60137");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            if (operacao == "Get")
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("api/Books/1");
                    var retorno = response.Content.ReadAsStringAsync().Result;

                    List<BookDto> books = new JavaScriptSerializer().Deserialize<List<BookDto>>(retorno);

                    if (response.StatusCode != HttpStatusCode.NotFound && books.Count == 0)
                    {
                        BookDto book = new JavaScriptSerializer().Deserialize<BookDto>(retorno);
                        Console.WriteLine(book.Title + " " + book.Author);
                    }
                    else
                    {
                        foreach (var item in books)
                        {
                            Console.WriteLine(item.Title + " " + item.Author);
                        }
                        Console.ReadLine();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "" + e.InnerException);
                }
            }

            else if (operacao == "Post")
            {
                Book b = new Book()
                {
                    Title = "Titulo",
                    Description = "Descrição",
                    Genre = "Genero",
                    Price = 10.00m,
                    PublishDate = DateTime.Now,
                    AuthorId = 1
                };

                try
                {
                    HttpResponseMessage response = await client.PostAsync<Book>("api/Books", b, new JsonMediaTypeFormatter());
                    Console.Write(response.Headers.Location.OriginalString);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            else if (operacao == "Put")
            {
                Book b = new Book()
                {
                    BookId = 1,
                    Title = "Titulo alterado",
                    Description = "Descrição",
                    Genre = "Genero",
                    Price = 10.00m,
                    PublishDate = DateTime.Now,
                    AuthorId = 1
                };

                HttpResponseMessage response = await client.PutAsync<Book>("api/Books", b, new JsonMediaTypeFormatter());
                Console.WriteLine(response.IsSuccessStatusCode);
            }

            else if (operacao == "Delete")
            {
                int id = 14;

                HttpResponseMessage response = await client.DeleteAsync("api/Books/" + id);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Cliente " + id + " excluído");
                }
            }
            Console.ReadLine();
        }
    }
}