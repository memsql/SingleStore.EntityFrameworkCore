using System.Collections.Generic;

namespace EntityFrameworkCore.SingleStore.IntegrationTests.Models
{
    public class Blog
    {
        public long Id { get; set; }
        public string Title { get; set; }

        public List<BlogPost> Posts { get; set; }
    }

	public class BlogPost
	{
		public long Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }

		public int BlogId { get; set; }
		public Blog Blog { get; set; }
	}
}
